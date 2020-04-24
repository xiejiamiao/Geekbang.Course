using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DimSum.Domain.Abstractions;

namespace DimSum.Infrastructure.Core
{
    public interface IRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        TEntity Add(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        bool Remove(Entity entity);

        Task<bool> RemoveAsync(Entity entity, CancellationToken cancellationToken = default);
    }

    public interface IRepository<TEntity, in TKey> : IRepository<TEntity> where TEntity : Entity<TKey>, IAggregateRoot
    {
        bool Delete(TKey id);

        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        TEntity Get(TKey id);

        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
