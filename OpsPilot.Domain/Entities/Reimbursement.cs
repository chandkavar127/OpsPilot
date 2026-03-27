using OpsPilot.Domain.Common;

namespace OpsPilot.Domain.Entities;

public class Reimbursement : BaseAuditableEntity
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string ExpenseType { get; set; } = string.Empty;

    public Request? Request { get; set; }
}