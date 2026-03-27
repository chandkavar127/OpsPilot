using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class Department : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ManagerUserId { get; set; }

    public ICollection<EmployeeProfile> Employees { get; set; } = new List<EmployeeProfile>();
}