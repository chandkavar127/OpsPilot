using OpsPilot.Domain.Common;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public class Ticket : BaseAuditableEntity
{
    public int Id { get; set; }
    public int EmployeeProfileId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public string? AssignedToUserId { get; set; }

    public EmployeeProfile? EmployeeProfile { get; set; }
}