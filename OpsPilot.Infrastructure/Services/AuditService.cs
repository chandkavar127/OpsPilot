using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IRepository<AuditLog> auditRepository, IUnitOfWork unitOfWork)
    {
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(string entityName, string entityId, string actionType, string performedBy, string changes, CancellationToken cancellationToken = default)
    {
        await _auditRepository.AddAsync(new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            ActionType = actionType,
            PerformedBy = performedBy,
            Changes = changes,
            TimestampUtc = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}