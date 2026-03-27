using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs;

public class RequestSummaryDto
{
    public int Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string PredictedStatus { get; set; } = string.Empty;
    public DateTime RequestedOnUtc { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public bool HasAttachment { get; set; }
}
