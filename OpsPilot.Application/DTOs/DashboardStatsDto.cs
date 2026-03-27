namespace OpsPilot.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalRequests { get; set; }
    public int PendingApprovals { get; set; }
    public int CompletedTasks { get; set; }
    public int OpenTickets { get; set; }
    public Dictionary<string, int> RequestStatusBreakdown { get; set; } = new();
}
