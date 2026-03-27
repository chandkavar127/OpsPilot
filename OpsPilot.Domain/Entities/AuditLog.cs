namespace OpsPilot.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string Changes { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}