---
title: .NET Core开发实战课程备忘(1) -- ASP.NET Core的启动流程
date: 2020/03/30
tag:
- ASP.NET Core
- 教程备忘
categories:
- .NET Core开发实战课程备忘
keywords:
- .NET Core
- ASP.NET Core
---

- [课程目标](#%e8%af%be%e7%a8%8b%e7%9b%ae%e6%a0%87)
- [创建项目](#%e5%88%9b%e5%bb%ba%e9%a1%b9%e7%9b%ae)
- [添加日志代码](#%e6%b7%bb%e5%8a%a0%e6%97%a5%e5%bf%97%e4%bb%a3%e7%a0%81)
- [运行结果](#%e8%bf%90%e8%a1%8c%e7%bb%93%e6%9e%9c)
- [结论](#%e7%bb%93%e8%ae%ba)
- [另外](#%e5%8f%a6%e5%a4%96)

# 课程目标
主要是掌握`ASP.NET Core`应用程序启动的流程，同时了解启动过程中哪些方法做了哪些操作

# 创建项目
创建`ASP.NET Core Web`项目，项目类型选择`API`，直接创建即可

# 添加日志代码
本实例不涉及任何业务代码，纯粹只是添加日志查看各个方法的调用流程

修改`Program.cs`，以下直接放修改后的代码
``` csharp
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace StartupDemo
{
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
                    webBuilder.UseStartup<Startup>();
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
```
修改`Startup.cs`，以下直接放修改后的代码
``` csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StartupDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Console.WriteLine("Startup.Ctor");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Startup.ConfigureServices");
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("Startup.Configure");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

# 运行结果
参照上面将代码修改完，直接运行代码，控制台将打印出以下信息 
```
ConfigureWebHostDefaults
ConfigureHostConfiguration
ConfigureAppConfiguration
Startup.Ctor
Startup.ConfigureServices
ConfigureServices
Startup.Configure
```

# 结论
以下为`ASP.NET Core`启动运行流程
* `ConfigureWebHostDefaults`：注册了应用程序必要的几个组件，比如说配置的组件、容器的组件等
* `ConfigureHostConfiguration`：配置程序启动时必要的配置，比如说程序启动时所需要监听的端口、需要监听的URL地址等，在这个过程可以嵌入我们自己的配置内容注入到配置的框架中去
* `ConfigureAppConfiguration`：嵌入我们自己的配置文件，供应用程序来读取，这些配置将来会在后续的应用程序执行过程中每个组件读取
* `ConfigureServices/ConfigureLogging/Startup/Startup.ConfigureServices`：这些都是往容器里面来注入我们的应用的组件
* `Startup.Configure`：注入中间件，处理HttpContext整个的请求过程

# 另外
从代码运行来看，`Startup.cs`这个类可以被整合到`Program.cs`里的`ConfigureWebHostDefaults`方法中，
可以直接`ConfigureWebHostDefaults`方法中直接使用`webBuilder`调用`ConfigureServices`和`Configure`两个方法
具体可以参考以下代码
``` csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StartupDemo
{
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
```
**但是为了代码结构更加合理，还是需要用`Startup`这个类来分离变动比较大的代码**

**通常是在Startup.ConfigureServices的方法里做服务注册，一般是Addxxx**

**在Startup.Configure决定注册那些中间件到处理过程中去**
