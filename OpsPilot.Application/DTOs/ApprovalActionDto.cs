using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs;

public class ApprovalActionDto
{
    public int RequestId { get; set; }
    public ApprovalDecision Decision { get; set; }
    public string Comments { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
