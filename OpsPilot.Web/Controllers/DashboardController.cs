using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Domain.Constants;
using OpsPilot.Infrastructure.Identity;
using OpsPilot.Web.Constants;
using OpsPilot.Web.ViewModels;

namespace OpsPilot.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IRequestService _requestService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IDashboardService dashboardService,
        IRequestService requestService,
        UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _requestService = requestService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var roles = await _userManager.GetRolesAsync(user);
        var model = new DashboardViewModel
        {
            RoleDashboard = roles.FirstOrDefault() ?? SystemRoles.Employee,
            Stats = await _dashboardService.GetDashboardStatsAsync(user.Id, roles),
            PendingApprovals = await _requestService.GetPendingApprovalsAsync(user.Id)
        };

        return View(model);
    }

    [Authorize(Roles = SystemRoles.Employee)]
    public Task<IActionResult> Employee() => LoadRoleDashboard(SystemRoles.Employee);

    [Authorize(Roles = SystemRoles.Manager)]
    public Task<IActionResult> Manager() => LoadRoleDashboard(SystemRoles.Manager);

    [Authorize(Roles = SystemRoles.Hr)]
    public Task<IActionResult> HR() => LoadRoleDashboard(SystemRoles.Hr);

    [Authorize(Roles = SystemRoles.ItSupport)]
    public Task<IActionResult> ITSupport() => LoadRoleDashboard(SystemRoles.ItSupport);

    [Authorize(Roles = SystemRoles.Finance)]
    public Task<IActionResult> Finance() => LoadRoleDashboard(SystemRoles.Finance);

    [Authorize(Roles = SystemRoles.Admin + "," + SystemRoles.SuperAdmin)]
    public Task<IActionResult> Admin() => LoadRoleDashboard(SystemRoles.Admin);

    private async Task<IActionResult> LoadRoleDashboard(string role)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(AppRoutes.LoginAction, AppRoutes.AccountController);

        var roles = await _userManager.GetRolesAsync(user);
        return View("Index", new DashboardViewModel
        {
            RoleDashboard = role,
            Stats = await _dashboardService.GetDashboardStatsAsync(user.Id, roles),
            PendingApprovals = await _requestService.GetPendingApprovalsAsync(user.Id)
        });
    }
}