using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Application.DTOs;
using OpsPilot.Domain.Constants;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;
using OpsPilot.Infrastructure.Identity;
using OpsPilot.Web.Constants;
using OpsPilot.Web.ViewModels;

namespace OpsPilot.Web.Controllers;

[Authorize]
public class RequestsController : Controller
{
    private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png", ".docx"];
    private const long MaxAttachmentSizeBytes = 5 * 1024 * 1024;

    private readonly IRepository<Request> _requestRepository;
    private readonly IRepository<EmployeeProfile> _employeeProfileRepository;
    private readonly IRepository<RequestType> _requestTypeRepository;
    private readonly IRepository<RequestStatusHistory> _requestStatusHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReportService _reportService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public RequestsController(
        IRepository<Request> requestRepository,
        IRepository<EmployeeProfile> employeeProfileRepository,
        IRepository<RequestType> requestTypeRepository,
        IRepository<RequestStatusHistory> requestStatusHistoryRepository,
        IUnitOfWork unitOfWork,
        IReportService reportService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment)
    {
        _requestRepository = requestRepository;
        _employeeProfileRepository = employeeProfileRepository;
        _requestTypeRepository = requestTypeRepository;
        _requestStatusHistoryRepository = requestStatusHistoryRepository;
        _unitOfWork = unitOfWork;
        _reportService = reportService;
        _userManager = userManager;
        _environment = environment;
    }

    [Authorize(Roles = SystemRoles.Employee)]
    public Task<IActionResult> Index() => MyRequests();

    [Authorize(Roles = SystemRoles.Employee)]
    public async Task<IActionResult> MyRequests()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var profile = (await _employeeProfileRepository.ListAsync(x => x.UserId == user.Id)).FirstOrDefault();
        if (profile is null)
        {
            return View(new List<Request>());
        }

        var requests = (await _requestRepository.ListAsync(x => x.EmployeeProfileId == profile.Id))
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return View(requests);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var request = (await _requestRepository.ListAsync(x => x.Id == id)).FirstOrDefault();
        if (request is null)
        {
            return NotFound();
        }

        var currentProfile = (await _employeeProfileRepository.ListAsync(x => x.UserId == user.Id)).FirstOrDefault();
        var canView = User.IsInRole(SystemRoles.Admin)
            || User.IsInRole(SystemRoles.SuperAdmin)
            || request.EmployeeProfile?.UserId == user.Id
            || (User.IsInRole(SystemRoles.Manager)
                && currentProfile is not null
                && request.EmployeeProfile?.ManagerProfileId == currentProfile.Id);

        if (!canView)
        {
            return Forbid();
        }

        var history = (await _requestStatusHistoryRepository.ListAsync(x => x.RequestId == id))
            .OrderBy(x => x.ActionDate)
            .ToList();

        var model = new RequestDetailsViewModel
        {
            Request = request,
            CurrentApprovalStage = GetCurrentApprovalStage(request.Status),
            ManagerActionByName = await ResolveUserNameAsync(request.ManagerApprovedById),
            AdminActionByName = await ResolveUserNameAsync(request.AdminApprovedById),
            History = await BuildHistoryTimelineAsync(history)
        };

        return View(model);
    }

    [Authorize(Roles = SystemRoles.Employee)]
    public async Task<IActionResult> Create()
    {
        ViewBag.RequestTypes = await _requestTypeRepository.ListAsync();
        return View(new CreateRequestDto());
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.Employee)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRequestDto dto, IFormFile? attachment)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.RequestTypes = await _requestTypeRepository.ListAsync();
            return View(dto);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var profile = (await _employeeProfileRepository.ListAsync(x => x.UserId == user.Id)).FirstOrDefault();
        if (profile is null)
        {
            ModelState.AddModelError(string.Empty, "Employee profile is missing.");
            ViewBag.RequestTypes = await _requestTypeRepository.ListAsync();
            return View(dto);
        }

        if (attachment is not null && attachment.Length > 0)
        {
            var extension = Path.GetExtension(attachment.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(string.Empty, "Only PDF, JPG, PNG, and DOCX files are allowed.");
                ViewBag.RequestTypes = await _requestTypeRepository.ListAsync();
                return View(dto);
            }

            if (attachment.Length > MaxAttachmentSizeBytes)
            {
                ModelState.AddModelError(string.Empty, "Attachment must be smaller than 5 MB.");
                ViewBag.RequestTypes = await _requestTypeRepository.ListAsync();
                return View(dto);
            }

            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(attachment.FileName)}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await attachment.CopyToAsync(stream);
            dto.AttachmentPath = $"/uploads/{fileName}";
        }

        var duplicateExists = (await _requestRepository.ListAsync(x =>
            x.EmployeeProfileId == profile.Id
            && x.Title == dto.Title
            && x.RequestTypeId == dto.RequestTypeId
            && x.CreatedAt >= DateTime.UtcNow.AddDays(-7))).Any();

        if (duplicateExists)
        {
            ModelState.AddModelError(string.Empty, "A similar request was already submitted in the last 7 days.");
            ViewBag.RequestTypes = await _requestTypeRepository.ListAsync();
            return View(dto);
        }

        var request = new Request
        {
            Title = dto.Title,
            Description = dto.Description,
            RequestTypeId = dto.RequestTypeId,
            EmployeeProfileId = profile.Id,
            AttachmentPath = dto.AttachmentPath,
            TargetDateUtc = dto.TargetDateUtc,
            Status = RequestStatus.PendingManagerApproval,
            CreatedAt = DateTime.UtcNow,
            RequestedOnUtc = DateTime.UtcNow,
            CreatedBy = user.Id
        };

        await _requestRepository.AddAsync(request);
        await _requestStatusHistoryRepository.AddAsync(new RequestStatusHistory
        {
            Request = request,
            Status = RequestStatus.PendingManagerApproval,
            ActionByUserId = user.Id,
            ActionByRole = SystemRoles.Employee,
            Remarks = "Request submitted",
            ActionDate = DateTime.UtcNow,
            CreatedBy = user.Id
        });
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(MyRequests));
    }

    [Authorize(Roles = SystemRoles.Manager)]
    public Task<IActionResult> Pending() => ManagerPendingRequests();

    [Authorize(Roles = SystemRoles.Manager)]
    public async Task<IActionResult> ManagerPendingRequests()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var managerProfile = (await _employeeProfileRepository.ListAsync(x => x.UserId == user.Id)).FirstOrDefault();
        if (managerProfile is null)
        {
            return View(new List<Request>());
        }

        var directReportIds = (await _employeeProfileRepository.ListAsync(x => x.ManagerProfileId == managerProfile.Id))
            .Select(x => x.Id)
            .ToList();

        var requests = (await _requestRepository.ListAsync(x =>
                x.Status == RequestStatus.PendingManagerApproval
                && directReportIds.Contains(x.EmployeeProfileId)))
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return View(requests);
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.Manager)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManagerApprove(int requestId, string? managerRemarks)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var managerProfile = (await _employeeProfileRepository.ListAsync(x => x.UserId == user.Id)).FirstOrDefault();
        if (managerProfile is null)
        {
            return RedirectToAction(nameof(ManagerPendingRequests));
        }

        var request = (await _requestRepository.ListAsync(x => x.Id == requestId)).FirstOrDefault();
        if (request is null || request.Status != RequestStatus.PendingManagerApproval)
        {
            return RedirectToAction(nameof(ManagerPendingRequests));
        }

        if (request.EmployeeProfile?.ManagerProfileId != managerProfile.Id)
        {
            return Forbid();
        }

        request.Status = RequestStatus.PendingAdminApproval;
        request.ManagerApprovedById = user.Id;
        request.ManagerActionDate = DateTime.UtcNow;
        request.ManagerRemarks = managerRemarks;
        request.UpdatedBy = user.Id;
        _requestRepository.Update(request);

        await _requestStatusHistoryRepository.AddAsync(new RequestStatusHistory
        {
            RequestId = request.Id,
            Status = RequestStatus.PendingAdminApproval,
            ActionByUserId = user.Id,
            ActionByRole = SystemRoles.Manager,
            Remarks = string.IsNullOrWhiteSpace(managerRemarks) ? "Manager approved" : managerRemarks,
            ActionDate = DateTime.UtcNow,
            CreatedBy = user.Id
        });

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(ManagerPendingRequests));
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.Manager)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManagerReject(int requestId, string? managerRemarks)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var managerProfile = (await _employeeProfileRepository.ListAsync(x => x.UserId == user.Id)).FirstOrDefault();
        if (managerProfile is null)
        {
            return RedirectToAction(nameof(ManagerPendingRequests));
        }

        var request = (await _requestRepository.ListAsync(x => x.Id == requestId)).FirstOrDefault();
        if (request is null || request.Status != RequestStatus.PendingManagerApproval)
        {
            return RedirectToAction(nameof(ManagerPendingRequests));
        }

        if (request.EmployeeProfile?.ManagerProfileId != managerProfile.Id)
        {
            return Forbid();
        }

        request.Status = RequestStatus.Rejected;
        request.ManagerApprovedById = user.Id;
        request.ManagerActionDate = DateTime.UtcNow;
        request.ManagerRemarks = managerRemarks;
        request.UpdatedBy = user.Id;
        _requestRepository.Update(request);

        await _requestStatusHistoryRepository.AddAsync(new RequestStatusHistory
        {
            RequestId = request.Id,
            Status = RequestStatus.Rejected,
            ActionByUserId = user.Id,
            ActionByRole = SystemRoles.Manager,
            Remarks = string.IsNullOrWhiteSpace(managerRemarks) ? "Manager rejected" : managerRemarks,
            ActionDate = DateTime.UtcNow,
            CreatedBy = user.Id
        });

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(ManagerPendingRequests));
    }

    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> AdminPendingRequests()
    {
        var requests = (await _requestRepository.ListAsync(x => x.Status == RequestStatus.PendingAdminApproval))
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return View(requests);
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminApprove(int requestId, string? adminRemarks)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var request = (await _requestRepository.ListAsync(x => x.Id == requestId)).FirstOrDefault();
        if (request is null || request.Status != RequestStatus.PendingAdminApproval)
        {
            return RedirectToAction(nameof(AdminPendingRequests));
        }

        request.Status = RequestStatus.Approved;
        request.AdminApprovedById = user.Id;
        request.AdminActionDate = DateTime.UtcNow;
        request.AdminRemarks = adminRemarks;
        request.UpdatedBy = user.Id;
        _requestRepository.Update(request);

        await _requestStatusHistoryRepository.AddAsync(new RequestStatusHistory
        {
            RequestId = request.Id,
            Status = RequestStatus.Approved,
            ActionByUserId = user.Id,
            ActionByRole = SystemRoles.Admin,
            Remarks = string.IsNullOrWhiteSpace(adminRemarks) ? "Admin approved" : adminRemarks,
            ActionDate = DateTime.UtcNow,
            CreatedBy = user.Id
        });

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(AdminPendingRequests));
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminReject(int requestId, string? adminRemarks)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var request = (await _requestRepository.ListAsync(x => x.Id == requestId)).FirstOrDefault();
        if (request is null || request.Status != RequestStatus.PendingAdminApproval)
        {
            return RedirectToAction(nameof(AdminPendingRequests));
        }

        request.Status = RequestStatus.Rejected;
        request.AdminApprovedById = user.Id;
        request.AdminActionDate = DateTime.UtcNow;
        request.AdminRemarks = adminRemarks;
        request.UpdatedBy = user.Id;
        _requestRepository.Update(request);

        await _requestStatusHistoryRepository.AddAsync(new RequestStatusHistory
        {
            RequestId = request.Id,
            Status = RequestStatus.Rejected,
            ActionByUserId = user.Id,
            ActionByRole = SystemRoles.Admin,
            Remarks = string.IsNullOrWhiteSpace(adminRemarks) ? "Admin rejected" : adminRemarks,
            ActionDate = DateTime.UtcNow,
            CreatedBy = user.Id
        });

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(AdminPendingRequests));
    }

    [Authorize(Roles = SystemRoles.Admin + "," + SystemRoles.SuperAdmin)]
    public async Task<IActionResult> ExportExcel()
    {
        var file = await _reportService.ExportRequestsAsync();
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "requests-report.xlsx");
    }

    private static string GetCurrentApprovalStage(RequestStatus status)
    {
        return status switch
        {
            RequestStatus.PendingManagerApproval => "Pending with Manager",
            RequestStatus.PendingAdminApproval => "Pending with Admin",
            RequestStatus.Approved => "Completed",
            RequestStatus.Rejected => "Completed",
            _ => "Unknown"
        };
    }

    private async Task<string?> ResolveUserNameAsync(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId);
        return user?.FullName ?? user?.Email ?? userId;
    }

    private async Task<List<RequestStatusHistoryTimelineItem>> BuildHistoryTimelineAsync(List<RequestStatusHistory> history)
    {
        var timeline = new List<RequestStatusHistoryTimelineItem>();
        foreach (var item in history)
        {
            timeline.Add(new RequestStatusHistoryTimelineItem
            {
                Status = item.Status.ToString(),
                ActionByRole = item.ActionByRole,
                ActionByName = await ResolveUserNameAsync(item.ActionByUserId) ?? item.ActionByUserId,
                Remarks = item.Remarks,
                ActionDate = item.ActionDate
            });
        }

        return timeline;
    }
}