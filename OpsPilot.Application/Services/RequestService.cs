using AutoMapper;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Application.DTOs;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.Services;

public class RequestService : IRequestService
{
    private readonly IRepository<Request> _requestRepository;
    private readonly IRepository<EmployeeProfile> _employeeRepository;
    private readonly IRepository<RequestType> _requestTypeRepository;
    private readonly IRepository<Reimbursement> _reimbursementRepository;
    private readonly IRepository<Asset> _assetRepository;
    private readonly IRepository<ApprovalStep> _approvalStepRepository;
    private readonly IWorkflowService _workflowService;
    private readonly ISmartAutomationService _smartAutomationService;
    private readonly INotificationService _notificationService;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RequestService(
        IRepository<Request> requestRepository,
        IRepository<EmployeeProfile> employeeRepository,
        IRepository<RequestType> requestTypeRepository,
        IRepository<Reimbursement> reimbursementRepository,
        IRepository<Asset> assetRepository,
        IRepository<ApprovalStep> approvalStepRepository,
        IWorkflowService workflowService,
        ISmartAutomationService smartAutomationService,
        INotificationService notificationService,
        IAuditService auditService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _employeeRepository = employeeRepository;
        _requestTypeRepository = requestTypeRepository;
        _reimbursementRepository = reimbursementRepository;
        _assetRepository = assetRepository;
        _approvalStepRepository = approvalStepRepository;
        _workflowService = workflowService;
        _smartAutomationService = smartAutomationService;
        _notificationService = notificationService;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> CreateRequestAsync(CreateRequestDto dto, string userId, CancellationToken cancellationToken = default)
    {
        var employee = (await _employeeRepository.ListAsync(x => x.UserId == userId, cancellationToken)).FirstOrDefault();
        if (employee is null)
        {
            throw new InvalidOperationException("Employee profile not found.");
        }

        var requestType = await _requestTypeRepository.GetByIdAsync(dto.RequestTypeId, cancellationToken);
        if (requestType is null)
        {
            throw new InvalidOperationException("Request type not found.");
        }

        var request = new Request
        {
            RequestTypeId = dto.RequestTypeId,
            EmployeeProfileId = employee.Id,
            Title = dto.Title,
            Description = dto.Description,
            TargetDateUtc = dto.TargetDateUtc,
            AttachmentPath = dto.AttachmentPath,
            Status = RequestStatus.PendingManagerApproval,
            CreatedBy = userId,
            PredictedStatus = "Pending Review"
        };

        if (await _smartAutomationService.IsDuplicateRequestAsync(request, cancellationToken))
        {
            throw new InvalidOperationException("Similar request already exists within the last 7 days.");
        }

        request.PredictedStatus = _smartAutomationService.PredictRequestStatus(request);
        await _requestRepository.AddAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (requestType.Code.Equals("REIMBURSE", StringComparison.OrdinalIgnoreCase) && dto.ReimbursementAmount.HasValue)
        {
            await _reimbursementRepository.AddAsync(new Reimbursement
            {
                RequestId = request.Id,
                Amount = dto.ReimbursementAmount.Value,
                ExpenseType = dto.ExpenseType ?? "General",
                CreatedBy = userId
            }, cancellationToken);
        }

        if (requestType.Code.Equals("ASSET", StringComparison.OrdinalIgnoreCase))
        {
            await _assetRepository.AddAsync(new Asset
            {
                RequestId = request.Id,
                AssetType = dto.AssetType ?? "General",
                AssetName = dto.AssetName ?? dto.Title,
                CreatedBy = userId
            }, cancellationToken);
        }

        var steps = await _workflowService.BuildApprovalStepsAsync(request, cancellationToken);
        foreach (var step in steps)
        {
            await _approvalStepRepository.AddAsync(step, cancellationToken);
            await _notificationService.NotifyAsync(step.ApproverUserId, "New Approval", $"Request #{request.Id} requires your approval.", cancellationToken);
        }

        await _auditService.LogAsync(nameof(Request), request.Id.ToString(), "Created", userId, request.Title, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return request.Id;
    }

    public async Task<List<RequestSummaryDto>> GetEmployeeRequestsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.ListAsync(x => x.EmployeeProfile != null && x.EmployeeProfile.UserId == userId, cancellationToken);
        return _mapper.Map<List<RequestSummaryDto>>(requests.OrderByDescending(x => x.RequestedOnUtc));
    }

    public async Task<List<RequestSummaryDto>> GetPendingApprovalsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var requestIds = (await _approvalStepRepository.ListAsync(
            x => x.ApproverUserId == userId && x.Decision == ApprovalDecision.Pending,
            cancellationToken)).Select(x => x.RequestId).Distinct().ToHashSet();

        var requests = await _requestRepository.ListAsync(x => requestIds.Contains(x.Id), cancellationToken);
        return _mapper.Map<List<RequestSummaryDto>>(requests.OrderByDescending(x => x.RequestedOnUtc));
    }

    public async Task<bool> ExecuteApprovalAsync(ApprovalActionDto dto, CancellationToken cancellationToken = default)
    {
        var step = (await _approvalStepRepository.ListAsync(
            x => x.RequestId == dto.RequestId && x.ApproverUserId == dto.UserId && x.Decision == ApprovalDecision.Pending,
            cancellationToken)).OrderBy(x => x.Sequence).FirstOrDefault();

        if (step is null)
        {
            return false;
        }

        step.Decision = dto.Decision;
        step.Comments = dto.Comments;
        step.ActionedAtUtc = DateTime.UtcNow;
        step.UpdatedBy = dto.UserId;
        step.UpdatedAtUtc = DateTime.UtcNow;
        _approvalStepRepository.Update(step);

        var request = await _requestRepository.GetByIdAsync(dto.RequestId, cancellationToken);
        if (request is null)
        {
            return false;
        }

        var allSteps = await _approvalStepRepository.ListAsync(x => x.RequestId == dto.RequestId, cancellationToken);
        if (dto.Decision == ApprovalDecision.Rejected)
        {
            request.Status = RequestStatus.Rejected;
        }
        else if (allSteps.All(x => x.Decision == ApprovalDecision.Approved))
        {
            request.Status = RequestStatus.Approved;
        }
        else
        {
            request.Status = RequestStatus.PendingAdminApproval;
        }

        request.UpdatedAtUtc = DateTime.UtcNow;
        request.UpdatedBy = dto.UserId;
        _requestRepository.Update(request);

        await _auditService.LogAsync(nameof(Request), request.Id.ToString(), "ApprovalAction", dto.UserId, dto.Decision.ToString(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
