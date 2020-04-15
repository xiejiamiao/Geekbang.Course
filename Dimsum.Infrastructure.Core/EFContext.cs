using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dimsum.Infrastructure.Core.Extensions;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dimsum.Infrastructure.Core
{
    // ReSharper disable once InconsistentNaming
    public class EFContext : DbContext, IUnitOfWork, ITransaction
    {
        private readonly IMediator _mediator;
        private readonly ICapPublisher _capBus;

        public EFContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options)
        {
            _mediator = mediator;
            _capBus = capBus;
        }

        #region IUnitOfWork

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            await _mediator.DispatchDomainEventAsync(this);
            return true;
        }
        #endregion


        #region ITransaction

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;
        public async Task<IDbContextTransaction> BeginTransaction()
        {
            await Task.CompletedTask;
            if (_currentTransaction != null) return null;
            _currentTransaction = Database.BeginTransaction(_capBus, autoCommit: false);
            return _currentTransaction;
        }

        public async Task CommitTransaction(IDbContextTransaction transaction)
        {
            if(transaction==null)  throw new ArgumentNullException(nameof(transaction));
            if(transaction!=_currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransaction();
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

        public async Task RollbackTransaction()
        {
            try
            {
                await _currentTransaction.RollbackAsync();
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
