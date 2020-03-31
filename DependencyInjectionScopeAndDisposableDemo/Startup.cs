using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionScopeAndDisposableDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionScopeAndDisposableDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 注册一个瞬时服务
            //services.AddTransient<IOrderService, DisposableOrderService>();

            // 使用工厂注入方式注册一个Scope服务
            //services.AddScoped<IOrderService>(serviceProvider => new DisposableOrderService());

            // 使用工厂注入方式注册一个Singleton服务
            //services.AddSingleton<IOrderService>(serviceProvider => new DisposableOrderService());

            // 自己new一个对象然后注入到容器中去
            // var orderService = new DisposableOrderService();
            // services.AddSingleton<IOrderService>(orderService);

            // 注册一个瞬时服务
            services.AddTransient<IOrderService, DisposableOrderService>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // 从根容器获取瞬时服务对象，由于根容器只会在应用程序整个退出时回收，这就意味着即使这是个瞬时服务，但是应用程序不退出，这些对象会一直积累在应用程序内不得释放
            var s = app.ApplicationServices.GetService<IOrderService>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
