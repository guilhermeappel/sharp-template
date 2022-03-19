using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Infrastructure.Data.Repositories;

public class RepositoryBase<TEntity> : IAsyncDisposable, IRepositoryBase<TEntity> where TEntity : BaseEntity
{
    protected readonly DbSet<TEntity> DbSet;
    private readonly SharpTemplateContext _context;

    public RepositoryBase(IDbContextFactory<SharpTemplateContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
        DbSet = _context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (filter != null)
            query = query
                .Where(filter);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public ValueTask DisposeAsync() => _context.DisposeAsync();
}
