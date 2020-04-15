using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dimsum.Domain.Abstractions;

namespace Dimsum.Infrastructure.Core
{
    public abstract class Repository<TEntity, TDbContext> : IRepository<TEntity> 
        where TEntity : Entity, IAggregateRoot
        where TDbContext : EFContext
    {
        protected virtual TDbContext DbContext { get; set; }

        public Repository(TDbContext context)
        {
            this.DbContext = context;
        }

        public IUnitOfWork UnitOfWork => this.DbContext;

        public virtual TEntity Add(TEntity entity)
        {
            return DbContext.Add(entity).Entity;
        }

        
        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return (await DbContext.AddAsync(entity, cancellationToken)).Entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            return DbContext.Update(entity).Entity;
        }

        

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Update(entity);
        }

        public virtual bool Remove(TEntity entity)
        {
            DbContext.Remove(entity);
            return true;
        }

        public virtual async Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Remove(entity);
        }
    }


    public abstract class Repository<TEntity, TKey, TDbContext> : Repository<TEntity, TDbContext>, IRepository<TEntity, TKey> 
        where TEntity : Entity<TKey>, IAggregateRoot 
        where TDbContext : EFContext
    {
        protected Repository(TDbContext context) : base(context)
        {
        }

        public virtual bool Delete(TKey id)
        {
            var entity = DbContext.Find<TEntity>(id);
            if (entity == null) return false;
            DbContext.Remove(entity);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await DbContext.FindAsync<TEntity>(id, cancellationToken);
            if (entity == null) return false;
            DbContext.Remove(entity);
            return true;
        }

        public virtual TEntity Get(TKey id)
        {
            return DbContext.Find<TEntity>(id);
        }

        public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await DbContext.FindAsync<TEntity>(id, cancellationToken);
        }


    }
}
