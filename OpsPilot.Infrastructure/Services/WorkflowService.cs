using Microsoft.AspNetCore.Identity;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Infrastructure.Identity;

namespace OpsPilot.Infrastructure.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IRepository<ApprovalFlow> _flowRepository;
    private readonly IRepository<EmployeeProfile> _employeeRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public WorkflowService(
        IRepository<ApprovalFlow> flowRepository,
        IRepository<EmployeeProfile> employeeRepository,
        UserManager<ApplicationUser> userManager)
    {
        _flowRepository = flowRepository;
        _employeeRepository = employeeRepository;
        _userManager = userManager;
    }

    public async Task<List<ApprovalStep>> BuildApprovalStepsAsync(Request request, CancellationToken cancellationToken = default)
    {
        var flow = (await _flowRepository.ListAsync(
            x => x.RequestTypeId == request.RequestTypeId && x.IsActive,
            cancellationToken)).FirstOrDefault();

        if (flow is null)
        {
            throw new InvalidOperationException("No active approval flow found for request type.");
        }

        var employee = (await _employeeRepository.ListAsync(x => x.Id == request.EmployeeProfileId, cancellationToken)).FirstOrDefault();
        if (employee is null)
        {
            throw new InvalidOperationException("Employee not found while generating workflow.");
        }

        var stepTemplates = flow.StepTemplates.OrderBy(x => x.Sequence).ToList();
        var steps = new List<ApprovalStep>();

        foreach (var template in stepTemplates)
        {
            string? approverUserId;

            if (template.IsDepartmentManager && employee.Department is not null)
            {
                approverUserId = employee.Department.ManagerUserId;
            }
            else
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(template.RoleName);
                approverUserId = usersInRole.FirstOrDefault()?.Id;
            }

            if (string.IsNullOrWhiteSpace(approverUserId))
            {
                throw new InvalidOperationException($"No approver available for role {template.RoleName}.");
            }

            steps.Add(new ApprovalStep
            {
                RequestId = request.Id,
                Sequence = template.Sequence,
                ApproverRole = template.RoleName,
                ApproverUserId = approverUserId,
                CreatedBy = request.CreatedBy
            });
        }

        return steps;
    }
}