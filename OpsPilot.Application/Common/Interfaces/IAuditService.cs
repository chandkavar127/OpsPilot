namespace OpsPilot.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(string entityName, string entityId, string actionType, string performedBy, string changes, CancellationToken cancellationToken = default);
}
