using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiddlewareDemo.Middlewares.MyMiddleware;

namespace MiddlewareDemo
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
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /* ֱ��ע��ί�е��м��
            app.Use(async (context, next) =>
            {
                await next();
                await context.Response.WriteAsync("Hello world -- my middleware");
            });
            */

            /*
            // Map�Ƕ��ض���·��ע���м��
            app.Map("/abc", builder =>
            {
                builder.Use(async (context, next) =>
                {
                    await next();
                    await context.Response.WriteAsync("Hello world abc -- from map middleware");
                });
            });
            */

            /*
            // ������жϱȽϸ��ӵ�ʱ��,��ʹ��MapWhen��ע���м��
            app.MapWhen(context => { return context.Request.Query.Keys.Contains("abc"); }, builder =>
            {
                builder.Use(async (context, next) =>
                {
                    await next();
                    await context.Response.WriteAsync("Hello world query abc -- from mapWhen middleware");
                });
            });
            */
            app.UseMyMiddleware();

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
