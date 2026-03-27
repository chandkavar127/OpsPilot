using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Domain.Constants;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Web.Controllers;

[Authorize(Roles = SystemRoles.Admin + "," + SystemRoles.SuperAdmin)]
public class AdminController : Controller
{
    private readonly IRepository<ApprovalFlow> _flowRepository;
    private readonly IRepository<RequestType> _requestTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminController(
        IRepository<ApprovalFlow> flowRepository,
        IRepository<RequestType> requestTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _flowRepository = flowRepository;
        _requestTypeRepository = requestTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Workflows()
    {
        ViewBag.RequestTypes = await _requestTypeRepository.ListAsync();
        var flows = await _flowRepository.ListAsync();
        return View(flows);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkflow(int requestTypeId, string flowName, string step1Role, string step2Role)
    {
        var flow = new ApprovalFlow
        {
            RequestTypeId = requestTypeId,
            Name = flowName,
            CreatedBy = User.Identity?.Name ?? "admin",
            StepTemplates =
            [
                new ApprovalStepTemplate { Sequence = 1, RoleName = step1Role, CreatedBy = User.Identity?.Name ?? "admin" },
                new ApprovalStepTemplate { Sequence = 2, RoleName = step2Role, CreatedBy = User.Identity?.Name ?? "admin" }
            ]
        };

        await _flowRepository.AddAsync(flow);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Workflows));
    }
}