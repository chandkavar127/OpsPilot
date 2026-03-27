using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class ApprovalStepTemplate : BaseAuditableEntity
{
    public int Id { get; set; }
    public int ApprovalFlowId { get; set; }
    public int Sequence { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsDepartmentManager { get; set; }

    public ApprovalFlow? ApprovalFlow { get; set; }
}