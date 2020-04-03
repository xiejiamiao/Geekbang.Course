using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StaticFilesDemo
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
            //services.AddDirectoryBrowser(); //����Ŀ¼���
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            /*
            �趨Ĭ���ļ�Ϊindex 
            ��:https://localhost:5001/=https://localhost:5001/index.html,https://localhost:5001/a=https://localhost:5001/a/index.html
            app.UseDefaultFiles();
            */

            /*
             ����Ŀ¼���
            app.UseDirectoryBrowser();
            */

            //Ĭ�Ͼ�̬�ļ���Ŀ¼Ϊwwwroot
            app.UseStaticFiles();

            app.MapWhen(context =>
            {
                return !context.Request.Path.Value.StartsWith("/api");
            }, appBuilder =>
            {
                var option = new RewriteOptions();
                option.AddRewrite(regex: ".*", replacement: "/index.html", skipRemainingRules: true);
                appBuilder.UseRewriter(option);
                appBuilder.UseStaticFiles();
            });

            /*
            // ˭ע��˭����ƥ��
            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = "/myfiles", //ָ���ĸ���������
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "file")) //ָ������·��
            });
            */

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
