using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DimSum.Infrastructure.Core.Extensions;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DimSum.Infrastructure.Core.Behaviors
{
    public class TransactionBehavior<TDbContext, TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TDbContext : EFContext
    {
        ILogger _logger;
        TDbContext _dbContext;
        ICapPublisher _capBus;

        public TransactionBehavior(ILogger logger, TDbContext dbContext, ICapPublisher capBus)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _capBus = capBus ?? throw new ArgumentNullException(nameof(capBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    await using var transaction = await _dbContext.BeginTransactionAsync();
                    using (_logger.BeginScope($"TransactionContext:{transaction.TransactionId}"))
                    {
                        _logger.LogInformation("------ 开始事务 {TransactionId} {@Command}", transaction.TransactionId, typeName, request);
                        response = await next();
                        _logger.LogInformation("------ 提交事务 {TransactionId} {@Command}", transaction.TransactionId, typeName, request);
                        await _dbContext.CommitTransactionAsync(transaction, cancellationToken);
                        transactionId = transaction.TransactionId;
                    }
                });
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(exception: e, "处理事务出错 {CommandName} ({@Command})", typeName, request);
                throw;
            }
        }
    }
}
