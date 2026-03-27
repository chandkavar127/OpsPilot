using OpsPilot.Application.DTOs;

namespace OpsPilot.Web.ViewModels;

public class DashboardViewModel
{
    public string RoleDashboard { get; set; } = "Employee";
    public DashboardStatsDto Stats { get; set; } = new();
    public List<RequestSummaryDto> PendingApprovals { get; set; } = new();
}