using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}