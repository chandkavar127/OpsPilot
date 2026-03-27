using OpsPilot.Domain.Common;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public class RequestStatusHistory : BaseAuditableEntity
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public RequestStatus Status { get; set; }
    public string ActionByUserId { get; set; } = string.Empty;
    public string ActionByRole { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;

    public Request? Request { get; set; }
}
