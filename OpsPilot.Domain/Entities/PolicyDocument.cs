using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class PolicyDocument : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}