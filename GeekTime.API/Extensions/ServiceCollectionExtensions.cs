using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekTime.API.Application.IntegrationEvents;
using GeekTime.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeekTime.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<DomainContext>(optionsAction);
        }

        public static IServiceCollection AddMySqlDomainContext(this IServiceCollection services, string connectionString)
        {
            return services.AddDomainContext(builder => { builder.UseMySql(connectionString); });
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISubscriberService, SubscriberService>();
            services.AddCap(options =>
            {
                options.UseEntityFramework<DomainContext>();
                options.UseRabbitMQ(r => { configuration.GetSection("RabbitMQ").Bind(r); });
                options.UseDashboard();
            });
            return services;
        }
    }
}
