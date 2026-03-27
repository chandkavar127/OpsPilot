using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class RequestType : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiresAttachment { get; set; }

    public ICollection<Request> Requests { get; set; } = new List<Request>();
    public ICollection<ApprovalFlow> ApprovalFlows { get; set; } = new List<ApprovalFlow>();
}