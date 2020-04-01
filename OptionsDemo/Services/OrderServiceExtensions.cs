using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            /*
            services.Configure<OrderServiceOptions>(configuration.GetSection("OrderService"));
            services.PostConfigure<OrderServiceOptions>(options =>
            {
                options.MaxOrderCount += 100;
            });
            */


            /*
             * //直接注册验证函数
            services.AddOptions<OrderServiceOptions>().Configure(options =>
            {
                configuration.GetSection("OrderService").Bind(options);
            }).Validate(options =>
            {
                return options.MaxOrderCount <= 100;

            },"MaxOrderCount 不能大于100");
            */

            
            //属性注入
            services.AddOptions<OrderServiceOptions>().Configure(options =>
            {
                configuration.GetSection("OrderService").Bind(options);
            }).ValidateDataAnnotations();
            

            //实现IValidateOptions<TOptions>
            //services.AddOptions<OrderServiceOptions>()
            //    .Configure(options => { configuration.GetSection("OrderService").Bind(options); }).Services
            //    .AddSingleton<IValidateOptions<OrderServiceOptions>, OrderServiceValidateOptions>();

            //services.AddScoped<IOrderService, OrderService>();
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
