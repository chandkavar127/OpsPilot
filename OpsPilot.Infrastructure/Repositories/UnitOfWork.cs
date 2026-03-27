using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OpsPilotDbContext _context;

    public UnitOfWork(OpsPilotDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}