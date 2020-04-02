---
title: .NET Core开发实战课程备忘(11) -- 日志框架：聊聊记日志的最佳姿势
date: 2020/04/02 11:12:00
tag:
- ASP.NET Core
- 教程备忘
categories:
- .NET Core开发实战课程备忘
keywords:
- .NET Core
- ASP.NET Core
---

- [概念](#%e6%a6%82%e5%bf%b5)
  - [依赖包](#%e4%be%9d%e8%b5%96%e5%8c%85)
  - [日志级别](#%e6%97%a5%e5%bf%97%e7%ba%a7%e5%88%ab)
  - [日志记录器](#%e6%97%a5%e5%bf%97%e8%ae%b0%e5%bd%95%e5%99%a8)
- [代码示例](#%e4%bb%a3%e7%a0%81%e7%a4%ba%e4%be%8b)
  - [创建项目](#%e5%88%9b%e5%bb%ba%e9%a1%b9%e7%9b%ae)
  - [手动创建日志记录器](#%e6%89%8b%e5%8a%a8%e5%88%9b%e5%bb%ba%e6%97%a5%e5%bf%97%e8%ae%b0%e5%bd%95%e5%99%a8)
  - [创建配置文件`appsettings.json`](#%e5%88%9b%e5%bb%ba%e9%85%8d%e7%bd%ae%e6%96%87%e4%bb%b6appsettingsjson)
  - [依赖注入创建日志记录器](#%e4%be%9d%e8%b5%96%e6%b3%a8%e5%85%a5%e5%88%9b%e5%bb%ba%e6%97%a5%e5%bf%97%e8%ae%b0%e5%bd%95%e5%99%a8)
- [关于日志作用域](#%e5%85%b3%e4%ba%8e%e6%97%a5%e5%bf%97%e4%bd%9c%e7%94%a8%e5%9f%9f)
  - [日志作用域的使用场景](#%e6%97%a5%e5%bf%97%e4%bd%9c%e7%94%a8%e5%9f%9f%e7%9a%84%e4%bd%bf%e7%94%a8%e5%9c%ba%e6%99%af)
  - [代码演示](#%e4%bb%a3%e7%a0%81%e6%bc%94%e7%a4%ba)
- [注意](#%e6%b3%a8%e6%84%8f)

# 概念
## 依赖包
对于输出到控制台的日志框架，主要依赖包有以下：
```
Microsoft.Extensions.Logging
Microsoft.Extensions.Logging.Console
Microsoft.Extensions.Logging.Debug
Microsoft.Extensions.Logging.TraceSource
```
## 日志级别
`.NET Core`中日志级别分`7`个级别，从低到高具体如下：
```
Trace -> Debug -> Information -> Warning -> Error -> Critical -> None
```
在配置中如果配置指定级别，则指定级别及以上的日志会被记录，低于指定级别的日志不会被记录，举例：配置项中指定级别为`Warning`，则只会记录`Warning`、`Error`、`Critical`这些日志，`Trace`、`Debug`、`Information`是不会记录下来，缺省默认配置为`Information`，如果指定为`None`级别则以为着不会有日志记录

## 日志记录器
记录日志的级别是属于某个日志记录器的，通过`ILoggerFactory`的对象方法`CreateLogger`创建一个日志记录器，传入参数就是这个日志记录去的名字，在配置相中对应配置该日志记录器的日志级别，即可指定要哪个日志记录器需要记录到哪些级别的日志

通常正常开发比较少去自己创建日志记录器，都是通过构造函数获取依赖注入的对象`ILogger<T>`，这样获取到的日志记录器的名字为`项目名.类型名称`，按照这个规律就可以自行配置各个日志记录器的日志记录级别，下面会有代码示例

# 代码示例
## 创建项目
创建名字为`LoggingSimpleDemo`的`控制台应用`，通过`nuget`引入以下五个包
```
Microsoft.Extensions.Configuration.Json
Microsoft.Extensions.Logging
Microsoft.Extensions.Logging.Console
Microsoft.Extensions.Logging.Debug
Microsoft.Extensions.Logging.TraceSource
```
这里引用`Microsoft.Extensions.Configuration.Json`这个包是因为记录日志的记录要从配置项中读取

## 手动创建日志记录器
修改`Program.Main`，代码如下：
``` csharp
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configurationRoot = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfigurationRoot>(p => configurationRoot);
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConfiguration(configurationRoot.GetSection("Logging"));
                builder.AddConsole();
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var loggerA = loggerFactory.CreateLogger("LoggerA");         
            loggerA.LogDebug(2001,"This is LoggerA -- logDebug");
            loggerA.LogInformation("This is LoggerA -- logInformation");
            loggerA.LogError(new Exception("LoggerA Error"),"This is LoggerA -- LogError");
        }
    }
}
```
1. `Main`方法中前三行代码是读取`appsettings.json`的配置项
2. `new ServiceCollection()`是创建一个服务容器，然后将上面读到的服务注册到服务容器中去，有服务容器来管理配置项服务生命周期
3. `serviceCollection.AddLogging()`是添加日志服务到服务容器中，`builder.AddConfiguration()`指的是日志使用的是配置框架里`Logging`的配置项，`builder.AddConsole()`是添加一个名字为`Console`的控制台日志到日志工厂中去
4. `serviceCollection.BuildServiceProvider()`是生成一个服务容器实例
5. `serviceProvider.GetService<ILoggerFactory>()`是从服务容器中获取一个`ILoggerFactory`的日志工厂对象
6. `loggerFactory.CreateLogger("LoggerA")`是创建一个名字为`LoggerA`的日志记录器
7. `LogDebug`、`LogInformation`、`LogError`则是记录对应级别的日志

## 创建配置文件`appsettings.json`
创建文件`appsettings.json`，具体内容如下：
``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "Console": {
      "LogLevel": {
        "LoggerA": "Trace"
      }
    }
  } 
}
```
将`appsettings.json`拷贝到输出目录(可参照`文件配置提供程序`里的操作)，运行项目，可以看到控制台打出以下信息：
```
dbug: LoggerA[2001]
      This is LoggerA -- logDebug
info: LoggerA[0]
      This is LoggerA -- logInformation
fail: LoggerA[0]
      This is LoggerA -- LogError
System.Exception: LoggerA Error
```
可以自行调整`appsettings.json`里的`LoggerA`配置来尝试不同级别的日志记录

## 依赖注入创建日志记录器
创建测试服务类`OrderService`，具体代码如下：
``` csharp
using System;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;

        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public void Show()
        {
            _logger.LogInformation("Show Time {time}",DateTime.Now);
        }
    }
}
```
在`Program.Main`方法中将`OrderService`注册进服务容器中，并获取出服务实例，调用`Show`方法，具体代码如下：
``` csharp
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configurationRoot = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfigurationRoot>(p => configurationRoot);
            serviceCollection.AddTransient<OrderService>();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConfiguration(configurationRoot.GetSection("Logging"));
                builder.AddConsole();
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            var loggerA = loggerFactory.CreateLogger("LoggerA");
            
            loggerA.LogDebug(2001,"This is LoggerA -- logDebug");
            loggerA.LogInformation("This is LoggerA -- logInformation");
            loggerA.LogError(new Exception("LoggerA Error"),"This is LoggerA -- LogError");

            var orderService = serviceProvider.GetService<OrderService>();
            orderService.Show();
            Console.ReadKey();
        }
    }
}
```
修改`appsettings.json`，具体内容如下：
``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "Console": {
      "LogLevel": {
        "LoggerA": "Trace",
        "LoggingSimpleDemo.OrderService": "Trace" 
      }
    }
  } 
}
```
运行项目，可以看到控制台输出日志信息
```
dbug: LoggerA[2001]
      This is LoggerA -- logDebug
info: LoggerA[0]
      This is LoggerA -- logInformation
fail: LoggerA[0]
      This is LoggerA -- LogError
System.Exception: LoggerA Error
info: LoggingSimpleDemo.OrderService[0]
      Show Time 04/02/2020 12:06:46
```
也可以自行调整`appsettings.json`里的配置来尝试不同级别的日志记录

# 关于日志作用域
## 日志作用域的使用场景
* 一个事务包含多条操作时
* 复杂流程的日志关联时
* 调用链追踪与请求处理过程对应时

## 代码演示
将上面获取日志记录器的代码注释掉，手动获取一个`Program`的日志记录器，然后创建日志作用域，具体代码如下
``` csharp
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configurationRoot = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfigurationRoot>(p => configurationRoot);
            serviceCollection.AddTransient<OrderService>();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConfiguration(configurationRoot.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();
            using (var scope = logger.BeginScope("scopeId={scopeId}",Guid.NewGuid()))
            {
                logger.LogTrace("This is Trace in scope");
                logger.LogInformation("This is Information in scope");
                logger.LogWarning("This is Warning in scope");
                logger.LogError("This is Error in scope");
            }

            Console.ReadKey();
        }
    }
}
```
在`appsettings.json`中添加`IncludeScopes`属性，具体代码如下：
``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "Console": {
      "IncludeScopes": true,
      "LogLevel": {
        "LoggerA": "Trace",
        "LoggingSimpleDemo.OrderService": "Trace"
      }
    }
  } 
}
```
运行代码，可以看到控制台打印出类似以下信息：
```
info: LoggingSimpleDemo.Program[0]
      => scopeId=40c5bb84-584d-4a4c-a56b-f3b66214e1ac
      This is Information in scope
warn: LoggingSimpleDemo.Program[0]
      => scopeId=40c5bb84-584d-4a4c-a56b-f3b66214e1ac
      This is Warning in scope
fail: LoggingSimpleDemo.Program[0]
      => scopeId=40c5bb84-584d-4a4c-a56b-f3b66214e1ac
      This is Error in scope
```
**在`ASP.NET Core`**项目中，要启用日志作用域，一样只需要在配置文件中新增`IncludeScopes`属性即可

# 注意
* 日志记录要避免敏感信息，如密码、密钥等
* 日志记录的时候最好用占位符的方式传参数，可以节省不必要的字符串拼接