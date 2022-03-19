using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IRepositoryBase<User> _repository;

    public UserRepository(IRepositoryBase<User> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>> filter = null, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAsync(filter, cancellationToken);
    }

    public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(entity, cancellationToken);
    }
}
