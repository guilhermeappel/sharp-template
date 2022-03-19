using Appel.SharpTemplate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>> filter = null, CancellationToken cancellationToken = default);
    Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(User entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(User entity, CancellationToken cancellationToken = default);
}
