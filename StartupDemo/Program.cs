using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StartupDemo
{
    /*
     * 课程目标：掌握ASP.NET Core的启动过程
     * 启动执行顺序
     * ConfigureWebHostDefaults    注册了应用程序必要的几个组件，比如说配置的组件、容器的组件等
     * ConfigureHostConfiguration  配置程序启动时必要的配置，比如说程序启动时所需要监听的端口、需要监听的URL地址等，在这个过程可以嵌入我们自己的配置内容注入到配置的框架中去
     * ConfigureAppConfiguration   嵌入我们自己的配置文件，供应用程序来读取，这些配置将来会在后续的应用程序执行过程中每个组件读取
     *
     * ConfigureServices/ConfigureLogging/Startup/Startup.ConfigureServices   这些都是往容器里面来注入我们的应用的组件
     * Startup.Configure  注入中间件，处理HttpContext整个的请求过程
     *
     * 整个过程甚至Startup这个类都不是必须的，可以直接ConfigureWebHostDefaults方法中直接使用webBuilder调用ConfigureServices和Configure两个方法
     * Startup这个类只是让代码结构更加合理
     *
     * 一般是在Startup.ConfigureServices的方法里做服务注册，一般是Addxxx
     * 在Startup.Configure决定注册那些中间件到处理过程去
     */



    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Console.WriteLine("ConfigureWebHostDefaults");
                    //webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureServices(services =>
                    {
                        Console.WriteLine("Program.ConfigureServices");
                        services.AddControllers();
                    });
                    webBuilder.Configure(app =>
                    {
                        Console.WriteLine("Program.Configure");

                        app.UseHttpsRedirection();

                        app.UseRouting();

                        app.UseAuthorization();

                        app.UseStaticFiles();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                })
                .ConfigureServices(service =>
                {
                    Console.WriteLine("ConfigureServices");
                })
                .ConfigureAppConfiguration(builder =>
                {
                    Console.WriteLine("ConfigureAppConfiguration");
                })
                .ConfigureHostConfiguration(builder =>
                {
                    Console.WriteLine("ConfigureHostConfiguration");
                });
    }
}
