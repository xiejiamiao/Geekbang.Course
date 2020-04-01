---
title: .NET Core开发实战课程备忘(10) -- 选项框架：服务组件集成配置的最佳实现
date: 2020/04/01
tag:
- ASP.NET Core
- 教程备忘
categories:
- .NET Core开发实战课程备忘
keywords:
- .NET Core
- ASP.NET Core
---

- [选项框架特性](#%e9%80%89%e9%a1%b9%e6%a1%86%e6%9e%b6%e7%89%b9%e6%80%a7)
- [设计原则](#%e8%ae%be%e8%ae%a1%e5%8e%9f%e5%88%99)
- [建议](#%e5%bb%ba%e8%ae%ae)
- [代码实现](#%e4%bb%a3%e7%a0%81%e5%ae%9e%e7%8e%b0)
  - [创建测试服务与测试服务对应的选项](#%e5%88%9b%e5%bb%ba%e6%b5%8b%e8%af%95%e6%9c%8d%e5%8a%a1%e4%b8%8e%e6%b5%8b%e8%af%95%e6%9c%8d%e5%8a%a1%e5%af%b9%e5%ba%94%e7%9a%84%e9%80%89%e9%a1%b9)
  - [修改`appsettings.json`](#%e4%bf%ae%e6%94%b9appsettingsjson)
  - [注册服务和选项](#%e6%b3%a8%e5%86%8c%e6%9c%8d%e5%8a%a1%e5%92%8c%e9%80%89%e9%a1%b9)
  - [获取服务进行测试](#%e8%8e%b7%e5%8f%96%e6%9c%8d%e5%8a%a1%e8%bf%9b%e8%a1%8c%e6%b5%8b%e8%af%95)
- [选项框架热更新](#%e9%80%89%e9%a1%b9%e6%a1%86%e6%9e%b6%e7%83%ad%e6%9b%b4%e6%96%b0)
  - [关键类型](#%e5%85%b3%e9%94%ae%e7%b1%bb%e5%9e%8b)
  - [代码示例](#%e4%bb%a3%e7%a0%81%e7%a4%ba%e4%be%8b)
    - [`IOptionsSnapshot`](#ioptionssnapshot)
    - [`IOptionsMonitor`](#ioptionsmonitor)
    - [`IOptionsMonitor`监听配置变动](#ioptionsmonitor%e7%9b%91%e5%90%ac%e9%85%8d%e7%bd%ae%e5%8f%98%e5%8a%a8)
- [优化代码结构](#%e4%bc%98%e5%8c%96%e4%bb%a3%e7%a0%81%e7%bb%93%e6%9e%84)
- [动态修改选项值](#%e5%8a%a8%e6%80%81%e4%bf%ae%e6%94%b9%e9%80%89%e9%a1%b9%e5%80%bc)
- [为选项添加验证逻辑](#%e4%b8%ba%e9%80%89%e9%a1%b9%e6%b7%bb%e5%8a%a0%e9%aa%8c%e8%af%81%e9%80%bb%e8%be%91)
  - [三种验证方法](#%e4%b8%89%e7%a7%8d%e9%aa%8c%e8%af%81%e6%96%b9%e6%b3%95)
  - [代码示例](#%e4%bb%a3%e7%a0%81%e7%a4%ba%e4%be%8b-1)
    - [直接注册验证](#%e7%9b%b4%e6%8e%a5%e6%b3%a8%e5%86%8c%e9%aa%8c%e8%af%81)
    - [实现`IValidateOptions<TOptions>`](#%e5%ae%9e%e7%8e%b0ivalidateoptionstoptions)
    - [使用属性标签的方式](#%e4%bd%bf%e7%94%a8%e5%b1%9e%e6%80%a7%e6%a0%87%e7%ad%be%e7%9a%84%e6%96%b9%e5%bc%8f)

# 选项框架特性
* 支持单例模式读取配置
* 支持快照
* 支持配置变更通知
* 支持运行时动态修改选项值
  
# 设计原则
* 接口分离原则(ISP)，我们的类不应该依赖它不使用的配置
* 关注点分离(SoC)，不同组件、服务、类之间的配置不应相互依赖或耦合

# 建议
* 为我们的服务设计`XXXOptions`
* 使用`IOptions<XXXOptions>`、`IOptionsSnapshot<XXXOptions>`、`IOptionsMonitor<XXXOptions>`作为服务构造函数的参数

# 代码实现
创建名为`OptionsDemo`的`ASP.NET Core`项目，类型为`API`
## 创建测试服务与测试服务对应的选项
创建`OrderService.cs`为了方便测试，这里将`IOrderService`、`OrderService`、`OrderServiceOption`都放在`OrderService.cs`文件中，具体代码如下：
``` csharp
using Microsoft.Extensions.Options;

namespace OptionsDemo.Services
{
    public interface IOrderService
    {
        int ShowMaxOrderCount();
    }

    public class OrderService:IOrderService
    {

        private readonly IOptions<OrderServiceOptions> _options;
        public OrderService(IOptions<OrderServiceOptions> options)
        {
            _options = options;
        }
        public int ShowMaxOrderCount()
        {
            return _options.Value.MaxOrderCount;
        }
    }

    public class OrderServiceOptions
    {
        public int MaxOrderCount { get; set; } = 100;
    }
}
```
## 修改`appsettings.json`
将项目中的`appsettings.json`的内容修改如下：
``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "OrderService": {
    "MaxOrderCount": 400
  } 
}
```
## 注册服务和选项
在`Startup.ConfigureServices`中添加以下代码：
``` csharp
services.Configure<OrderServiceOptions>(Configuration.GetSection("OrderService"));
services.AddScoped<IOrderService, OrderService>();
```
## 获取服务进行测试
修改`WeatherForecastController.Get`方法，具体代码如下：
``` csharp
[HttpGet]
public int Get([FromServices]IOrderService orderService)
{
    Console.WriteLine($"orderService.ShowMaxOrderCount:{orderService.ShowMaxOrderCount()}");
    return 1;
}
```
运行项目，可以访问`/WeatherForecast`，可以看到控制台打印出以下信息：
```
orderService.ShowMaxOrderCount:400
```
这里如果在项目运行时修改`appsettings.json`里配置项的值，重新访问`/WeatherForecast`地址，会发现控制台打印出来的值不会变，还依旧是400，这里就需要使用到下面的热更新。

# 选项框架热更新
## 关键类型
* 单例服务(Singleton)使用`IOptionsMonitor<out TOptions>`
* 范围作用域类型(Scope)使用`IOptionsSnapshot<out TOptions>`

## 代码示例

### `IOptionsSnapshot`
上个示例对`OrderService`注册的是`Scope`服务，所以这里先测试`Scope`对应的`IOptionsSnapshot`，将`OrderService`构造函数获取服务的类型修改为`IOptionsSnapshot<OrderServiceOptions>`，最终修改后`OrderService`类的代码如下：
``` csharp
public class OrderService:IOrderService
{
    private readonly IOptionsSnapshot<OrderServiceOptions> _options;
    public OrderService(IOptionsSnapshot<OrderServiceOptions> options)
    {
        _options = options;
    }
    public int ShowMaxOrderCount()
    {
        return _options.Value.MaxOrderCount;
    }
}
```
运行代码，访问`/WeatherForecast`，发现现在打印出来的是`appsettings.json`现有的值，对该配置项进行修改，保存之后重新访问`/WeatherForecast`，可以发现获取到的是新的值

### `IOptionsMonitor`
将`OrderService`构造函数获取服务的类型修改为`IOptionsMonitor<OrderServiceOptions>`，最终修改后`OrderService`类的代码如下：
``` csharp
public class OrderService:IOrderService
{
    private readonly IOptionsMonitor<OrderServiceOptions> _options;
    public OrderService(IOptionsMonitor<OrderServiceOptions> options)
    {
        _options = options;
    }
    public int ShowMaxOrderCount()
    {
        return _options.CurrentValue.MaxOrderCount;
    }
}
```
在`Startup.ConfigureServices`方法中将`OrderService`注册为单例模式，代码如下：
``` csharp
services.AddSingleton<IOrderService, OrderService>();
```
运行代码，访问`/WeatherForecast`，发现现在打印出来的是`appsettings.json`现有的值，对该配置项进行修改，保存之后重新访问`/WeatherForecast`，可以发现获取到的是新的值

### `IOptionsMonitor`监听配置变动
通过`IOptionsMonitor`对象的`OnChange`方法来注册配置变动操作，只需要在获取对象后注册相应操作即可，具体代码如下：
``` csharp
public class OrderService:IOrderService
{
    private readonly IOptionsMonitor<OrderServiceOptions> _options;
    public OrderService(IOptionsMonitor<OrderServiceOptions> options)
    {
        _options = options;
        this._options.OnChange(changedOptions =>
        {
            Console.WriteLine($"配置发生了变化,新值为:{changedOptions.MaxOrderCount}");
        });
    }
    public int ShowMaxOrderCount()
    {
        return _options.CurrentValue.MaxOrderCount;
    }
}
```
运行代码，修改`appsettings.json`的值，就可以看到控制台打印出类似以下信息：
```
配置发生了变化,新值为:100
```

# 优化代码结构
在开发中经常会出现服务与选项一起注册的情况，为了优化代码结构，一般会为统一将一个服务的注册放到`IServiceCollection`的扩展方法中去。创建`OrderServiceExtensions.cs`，具体代码如下：
``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OrderServiceOptions>(configuration.GetSection("OrderService"));
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
```
将`Startup.ConfigureServices`修改后代码如下：
``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddOrderService(Configuration);
    services.AddControllers();
}
```
运行之后可发现效果与上面一致
# 动态修改选项值
在注入选项之后，可动态对选项的值进行操作，这里以为`MaxOrderCount`的值增加`100`为例，修改`OrderServiceExtensions`，代码如下：
``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OrderServiceOptions>(configuration.GetSection("OrderService"));
            services.PostConfigure<OrderServiceOptions>(options =>
            {
                options.MaxOrderCount += 100;
            });
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
```
运行代码，可发现获得到的值比`appsettings.json`里的值增加`100`



# 为选项添加验证逻辑
## 三种验证方法
* 直接注册验证函数
* 实现`IValidateOptions<TOptions>`
* 使用`Microsoft.Extensions.Options.DataAnnotations`

## 代码示例
### 直接注册验证
修改`OrderServiceExtensions`，具体代码如下：
``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<OrderServiceOptions>().Configure(options =>
            {
                configuration.GetSection("OrderService").Bind(options);
            }).Validate(options =>
            {
                return options.MaxOrderCount <= 100;
            },"MaxOrderCount 不能大于100");
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}

```
修改`appsettings.json`对应的配置为`101`，然后运行代码，访问`/WeatherForecast`的时候会报错提示`MaxOrderCount 不能大于100`
### 实现`IValidateOptions<TOptions>`
修改`OrderService.cs`，在里面新增`OrderServiceValidateOptions`类，实现`IValidateOptions<OrderServiceOptions>`接口，具体代码如下：
``` csharp
public class OrderServiceValidateOptions : IValidateOptions<OrderServiceOptions>
{
    public ValidateOptionsResult Validate(string name, OrderServiceOptions options)
    {
        if (options.MaxOrderCount > 100)
        {
            return ValidateOptionsResult.Fail("MaxOrderCount 不能大于100");
        }
        else
        {
            return ValidateOptionsResult.Success;
        }
    }
}
```
修改`OrderServiceExtensions`，具体代码如下：
``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<OrderServiceOptions>()
                .Configure(options => { configuration.GetSection("OrderService").Bind(options); }).Services
                .AddSingleton<IValidateOptions<OrderServiceOptions>, OrderServiceValidateOptions>();
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
```
运行代码会得到与上一个示例一致的结果
### 使用属性标签的方式
修改`OrderServiceOptions`，具体代码如下：
``` csharp
public class OrderServiceOptions
{
    [Range(1,20)]
    public int MaxOrderCount { get; set; } = 100;
}
```
修改`OrderServiceExtensions`，具体代码如下：
``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<OrderServiceOptions>().Configure(options =>
            {
                configuration.GetSection("OrderService").Bind(options);
            }).ValidateDataAnnotations();
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
```
运行代码，访问`/WeatherForecast`会提示`The field MaxOrderCount must be between 1 and 20.`的错误信息