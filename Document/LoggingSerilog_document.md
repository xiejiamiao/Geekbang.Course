---
title: .NET Core开发实战课程备忘(12) -- 结构化日志组件Serilog：记录对查询分析友好的日志
date: 2020/04/02 16:12:00
tag:
- ASP.NET Core
- 教程备忘
categories:
- .NET Core开发实战课程备忘
keywords:
- .NET Core
- ASP.NET Core
---

- [结构化日志的好处](#%e7%bb%93%e6%9e%84%e5%8c%96%e6%97%a5%e5%bf%97%e7%9a%84%e5%a5%bd%e5%a4%84)
- [主要场景](#%e4%b8%bb%e8%a6%81%e5%9c%ba%e6%99%af)
- [代码实现](#%e4%bb%a3%e7%a0%81%e5%ae%9e%e7%8e%b0)
  - [创建项目](#%e5%88%9b%e5%bb%ba%e9%a1%b9%e7%9b%ae)
  - [用Serilog替换自带的日志框架](#%e7%94%a8serilog%e6%9b%bf%e6%8d%a2%e8%87%aa%e5%b8%a6%e7%9a%84%e6%97%a5%e5%bf%97%e6%a1%86%e6%9e%b6)
  - [修改`appsettings.json`文件](#%e4%bf%ae%e6%94%b9appsettingsjson%e6%96%87%e4%bb%b6)
  - [输出日志](#%e8%be%93%e5%87%ba%e6%97%a5%e5%bf%97)

# 结构化日志的好处
* 易于检索
* 易于分析统计

# 主要场景
* 实现日志告警
* 实现上下文的关联
* 实现与追踪系统集成

# 代码实现
## 创建项目
创建名字为`LoggingSerilogDemo`的`ASP.NET Core`项目，类型为`API`，因为使用的日志结构化组件为`Serilog`，所以需要引入以下的包:
```
Serilog.AspNetCore
```
## 用Serilog替换自带的日志框架
修改`Program`类，让Serilog替换掉`ASP.NET Core`自带的日志框架，具体代码如下：
``` csharp
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

namespace LoggingSerilogDemo
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.File(formatter: new CompactJsonFormatter(), "logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex,"Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog(dispose: true);
    }
}
```
代码解析：
1. 新建`Configuration`静态属性，直接读取`appsettings.json`的配置
2. `new LoggerConfiguration`是创建一个`Serilog`的`logger`对象，调的各种方法是进行日志配置
3. `CreateHostBuilder`的时候用`try...catch`包住是确保应用程序从开启到结束都有日志输出，包括启动失败，最后`Log.CloseAndFlush()`表示应用程序结束时会释放日志资源
4. `UseSerilog`表示正式引入`Serilog`组件，`dispose:true`表示应用程序关闭之后会自动释放日志资源

## 修改`appsettings.json`文件
在`appsettings.json`文件中新增`Serilog`节点，具体内容如下：
``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Information"
      }
    }
  },
  "AllowedHosts": "*"
}
```
## 输出日志
在`WeatherForecastController.Get`方法中新增以下代码：
``` csharp
_logger.LogInformation("This is information log");
```
运行项目，可以看到控制台打印出了结构化的日志，同时在项目根目录也有一个`logs`文件夹，进入文件夹可以看到日志文件
