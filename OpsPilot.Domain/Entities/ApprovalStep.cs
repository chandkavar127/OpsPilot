using OpsPilot.Domain.Common;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public class ApprovalStep : BaseAuditableEntity
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public int Sequence { get; set; }
    public string ApproverUserId { get; set; } = string.Empty;
    public string ApproverRole { get; set; } = string.Empty;
    public ApprovalDecision Decision { get; set; } = ApprovalDecision.Pending;
    public string? Comments { get; set; }
    public DateTime? ActionedAtUtc { get; set; }

    public Request? Request { get; set; }
}