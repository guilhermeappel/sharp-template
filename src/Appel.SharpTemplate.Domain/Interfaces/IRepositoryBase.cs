using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Domain.Interfaces;

public interface IRepositoryBase<TEntity>
{
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default);
    Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
