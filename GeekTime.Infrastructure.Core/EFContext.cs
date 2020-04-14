using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeekTime.Infrastructure.Core
{
    public class EFContext : DbContext, IUnitOfWork, ITransaction
    {

        public EFContext(DbContextOptions options) : base(options)
        {

        }

        #region IUnitOfWork

        public async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion



        #region ITransaction

        private IDbContextTransaction _currenTransaction;

        public IDbContextTransaction GetCurrentTransaction() => _currenTransaction;

        public bool HasActiveTransaction => _currenTransaction != null;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currenTransaction != null) return null;
            _currenTransaction = Database.BeginTransaction();
            return _currenTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currenTransaction)
                throw new InvalidOperationException(
                    $"Transaction {transaction.TransactionId} != CurrentTransaction {_currenTransaction.TransactionId}");

            try
            {
                await SaveChangeAsync();
                transaction.Commit();
            }
            catch (Exception e)
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currenTransaction != null)
                {
                    _currenTransaction.Dispose();
                    _currenTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currenTransaction?.Rollback();
            }
            finally
            {
                if (_currenTransaction != null)
                {
                    _currenTransaction.Dispose();
                    _currenTransaction = null;
                }
            }
        }

        #endregion
    }
}
