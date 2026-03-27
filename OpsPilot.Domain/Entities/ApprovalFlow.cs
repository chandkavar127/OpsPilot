using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class ApprovalFlow : BaseAuditableEntity
{
    public int Id { get; set; }
    public int RequestTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public RequestType? RequestType { get; set; }
    public ICollection<ApprovalStepTemplate> StepTemplates { get; set; } = new List<ApprovalStepTemplate>();
}