using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dimsum.Infrastructure.Core
{
    public interface ITransaction
    {
        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction { get; }

        Task<IDbContextTransaction> BeginTransaction();

        Task CommitTransaction(IDbContextTransaction transaction);

        Task RollbackTransaction();
    }
}
