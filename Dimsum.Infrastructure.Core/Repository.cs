using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DimSum.Domain.Abstractions;

namespace DimSum.Infrastructure.Core
{
    public abstract class Repository<TEntity, TDbContext> : IRepository<TEntity> 
        where TEntity : Entity, IAggregateRoot
        where TDbContext : EFContext
    {
        protected virtual TDbContext DbContext { get; set; }

        public IUnitOfWork UnitOfWork => DbContext;

        public Repository(TDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public TEntity Add(TEntity entity)
        {
            return DbContext.Add(entity).Entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return (await DbContext.AddAsync(entity, cancellationToken)).Entity;
        }

        public TEntity Update(TEntity entity)
        {
            return DbContext.Update(entity).Entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Update(entity);
        }

        public bool Remove(Entity entity)
        {
            DbContext.Remove(entity);
            return true;
        }

        public async Task<bool> RemoveAsync(Entity entity, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Remove(entity);
        }
    }

    public abstract class Repository<TEntity, TKey, TDbContext> : Repository<TEntity, TDbContext>,
        IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>, IAggregateRoot
        where TDbContext : EFContext
    {
        protected Repository(TDbContext dbContext) : base(dbContext)
        {
        }

        public bool Delete(TKey id)
        {
            var entity = DbContext.Find<TEntity>(id);
            if (entity == null) return false;
            DbContext.Remove(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await DbContext.FindAsync<TEntity>(id, cancellationToken);
            if (entity == null) return false;
            DbContext.Remove(entity);
            return true;
        }

        public TEntity Get(TKey id)
        {
            return DbContext.Find<TEntity>(id);
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await DbContext.FindAsync<TEntity>(id, cancellationToken);
        }
    }
}
