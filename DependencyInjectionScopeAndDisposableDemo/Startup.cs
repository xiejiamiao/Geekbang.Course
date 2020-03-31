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
            // ע��һ��˲ʱ����
            //services.AddTransient<IOrderService, DisposableOrderService>();

            // ʹ�ù���ע�뷽ʽע��һ��Scope����
            //services.AddScoped<IOrderService>(serviceProvider => new DisposableOrderService());

            // ʹ�ù���ע�뷽ʽע��һ��Singleton����
            //services.AddSingleton<IOrderService>(serviceProvider => new DisposableOrderService());

            // �Լ�newһ������Ȼ��ע�뵽������ȥ
            // var orderService = new DisposableOrderService();
            // services.AddSingleton<IOrderService>(orderService);

            // ע��һ��˲ʱ����
            services.AddTransient<IOrderService, DisposableOrderService>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // �Ӹ�������ȡ˲ʱ����������ڸ�����ֻ����Ӧ�ó��������˳�ʱ���գ������ζ�ż�ʹ���Ǹ�˲ʱ���񣬵���Ӧ�ó����˳�����Щ�����һֱ������Ӧ�ó����ڲ����ͷ�
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
