using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly OpsPilotDbContext _context;

    public Repository(OpsPilotDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync([id], cancellationToken);
    }

    public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _context.Set<T>().AsQueryable();
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}