using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs;

public class CreateTicketDto
{
    public int EmployeeProfileId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}
