using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 注册不同生命周期的服务（泛型注册服务）

            // 注册Singleton服务
            services.AddSingleton<IMySingletonService, MySingletonService>();
            // 注册Scope服务
            services.AddScoped<IMyScopeService, MyScopeService>();
            // 注册Transient服务
            services.AddTransient<IMyTransientService, MyTransientService>();

            #endregion

            #region 花式注册服务(以Singlton模式为例)

            services.AddSingleton<IOrderService>(new OrderService());  //直接注入实例
            services.AddSingleton<IOrderService>(serviceProvider =>
            {
                // 这里是可以使用serviceProvider这个入参的，也就意味着可以从容器里获取多个对象，然后进行组装，得到我们最终需要的实现实例，可以把工厂类设计的比较复杂
                // 比如说我们的实现类依赖了容器里面的另外一个类的情况
                // 或者我们期望用另外一个类来包装我们原有的实现的时候
                return new OrderServiceEx();
            }); //用工厂模式注入实例
            #endregion

            #region 尝试注册
            // 尝试注册是指如果服务已经注册过了，就不再注册
            services.TryAddSingleton<IOrderService,OrderService>();
            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
