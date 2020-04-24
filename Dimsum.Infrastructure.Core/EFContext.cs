using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DimSum.Infrastructure.Core.Extensions;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DimSum.Infrastructure.Core
{
    // ReSharper disable once InconsistentNaming
    public class EFContext : DbContext, IUnitOfWork, ITransaction
    {
        private readonly ICapPublisher _capBus;
        private readonly IMediator _mediator;

        public EFContext(DbContextOptions options,ICapPublisher capBus,IMediator mediator) : base(options)
        {
            _capBus = capBus;
            _mediator = mediator;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            await _mediator.DispatchDomainEventAsync(this);
            return result > 0;
        }

        #region ITransaction

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = Database.BeginTransaction(_capBus, autoCommit: true);
            return Task.FromResult(_currentTransaction);
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction,CancellationToken cancellationToken=default)
        {
            if(transaction == null) throw new ArgumentNullException(nameof(transaction));
            if(transaction!=_currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransaction(CancellationToken cancellationToken=default)
        {
            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion
    }
}
