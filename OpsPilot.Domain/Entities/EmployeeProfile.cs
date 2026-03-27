using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class EmployeeProfile : BaseAuditableEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int? ManagerProfileId { get; set; }
    public DateTime DateOfJoining { get; set; }

    public Department? Department { get; set; }
    public EmployeeProfile? ManagerProfile { get; set; }
    public ICollection<EmployeeProfile> DirectReports { get; set; } = new List<EmployeeProfile>();
    public ICollection<Request> Requests { get; set; } = new List<Request>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}