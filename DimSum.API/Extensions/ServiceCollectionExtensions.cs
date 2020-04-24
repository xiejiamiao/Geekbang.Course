using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DimSum.API.Application.IntegrationEvents;
using DimSum.Domain.OrderAggregate;
using DimSum.Infrastructure;
using DimSum.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DimSum.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRService(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainContextTransactionBehavior<,>));
            return services.AddMediatR(typeof(Order).Assembly, typeof(Program).Assembly);
        }

        public static IServiceCollection AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<DomainContext>(optionsAction);
        }

        public static IServiceCollection AddInMemoryDomainContext(this IServiceCollection services)
        {
            return services.AddDomainContext(builder => builder.UseInMemoryDatabase("domainContextDatabase"));
        }

        public static IServiceCollection AddMySqlDomainContext(this IServiceCollection services,string connectionString)
        {
            return services.AddDomainContext(builder => builder.UseMySql(connectionString));
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISubscribeService, SubscribeService>();
            services.AddCap(options =>
            {
                options.UseEntityFramework<DomainContext>();
                //options.UseMySql(configuration.GetValue<string>("Mysql"));
                options.UseRabbitMQ(r => { configuration.GetSection("RabbitMQ").Bind(r); });
                options.UseDashboard();
            });
            return services;
        }
    }
}
