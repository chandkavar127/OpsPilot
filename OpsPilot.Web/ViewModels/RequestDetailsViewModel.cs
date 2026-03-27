using OpsPilot.Domain.Entities;

namespace OpsPilot.Web.ViewModels;

public class RequestDetailsViewModel
{
    public Request Request { get; set; } = new();
    public string CurrentApprovalStage { get; set; } = string.Empty;
    public string? ManagerActionByName { get; set; }
    public string? AdminActionByName { get; set; }
    public List<RequestStatusHistoryTimelineItem> History { get; set; } = new();
}

public class RequestStatusHistoryTimelineItem
{
    public string Status { get; set; } = string.Empty;
    public string ActionByRole { get; set; } = string.Empty;
    public string ActionByName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime ActionDate { get; set; }
}
