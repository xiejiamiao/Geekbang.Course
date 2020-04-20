using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dimsum.Domain.OrderAggregate;
using Dimsum.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dimsum.API.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddEventBus(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddCap(options =>
            {
                options.UseEntityFramework<DomainContext>();
                options.UseRabbitMQ(r => { configuration.GetSection("RabbitMQ").Bind(r); });
            });
            return service;
        }

        public static IServiceCollection AddMediatRService(this IServiceCollection service)
        {
            service.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainContextTransactionBehavior<,>));
            service.AddMediatR(typeof(Order).Assembly, typeof(Program).Assembly);
            return service;
        }

        public static IServiceCollection AddMySqlDomainContext(this IServiceCollection service, string connectionString)
        {
            return service.AddDbContext<DomainContext>(builder => { builder.UseMySql(connectionString); });
        }

    }
}
