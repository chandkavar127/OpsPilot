using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class Asset : BaseAuditableEntity
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string AllocationStatus { get; set; } = "Requested";
    public DateTime? AllocatedOnUtc { get; set; }

    public Request? Request { get; set; }
}