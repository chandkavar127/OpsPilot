namespace OpsPilot.Application.DTOs;

public class CreateRequestDto
{
    public int RequestTypeId { get; set; }
    public int EmployeeProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? TargetDateUtc { get; set; }
    public string? AttachmentPath { get; set; }
    public decimal? ReimbursementAmount { get; set; }
    public string? ExpenseType { get; set; }
    public string? AssetType { get; set; }
    public string? AssetName { get; set; }
}
