using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OrderServiceOptions>(configuration.GetSection("OrderService"));
            services.PostConfigure<OrderServiceOptions>(options =>
            {
                options.MaxOrderCount += 100;
            });
            //services.AddScoped<IOrderService, OrderService>();
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
