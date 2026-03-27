using OpsPilot.Domain.Common;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public class Request : BaseAuditableEntity
{
    public int Id { get; set; }
    public int RequestTypeId { get; set; }
    public int EmployeeProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.PendingManagerApproval;
    public DateTime RequestedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ManagerApprovedById { get; set; }
    public string? AdminApprovedById { get; set; }
    public DateTime? ManagerActionDate { get; set; }
    public string? ManagerRemarks { get; set; }
    public DateTime? AdminActionDate { get; set; }
    public string? AdminRemarks { get; set; }
    public DateTime? TargetDateUtc { get; set; }
    public string? AttachmentPath { get; set; }
    public string PredictedStatus { get; set; } = string.Empty;

    public RequestType? RequestType { get; set; }
    public EmployeeProfile? EmployeeProfile { get; set; }
    public ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();
    public ICollection<RequestStatusHistory> StatusHistory { get; set; } = new List<RequestStatusHistory>();
    public Reimbursement? Reimbursement { get; set; }
    public Asset? Asset { get; set; }
}