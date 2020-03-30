---
title: .NET Core开发实战课程备忘(1) -- 依赖注入：良好架构的起点
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

- [为什么要使用依赖注入框架(IoC框架)](#%e4%b8%ba%e4%bb%80%e4%b9%88%e8%a6%81%e4%bd%bf%e7%94%a8%e4%be%9d%e8%b5%96%e6%b3%a8%e5%85%a5%e6%a1%86%e6%9e%b6ioc%e6%a1%86%e6%9e%b6)
  - [依赖注入框架组件包](#%e4%be%9d%e8%b5%96%e6%b3%a8%e5%85%a5%e6%a1%86%e6%9e%b6%e7%bb%84%e4%bb%b6%e5%8c%85)
  - [依赖注入框架核心类型](#%e4%be%9d%e8%b5%96%e6%b3%a8%e5%85%a5%e6%a1%86%e6%9e%b6%e6%a0%b8%e5%bf%83%e7%b1%bb%e5%9e%8b)
  - [生命周期](#%e7%94%9f%e5%91%bd%e5%91%a8%e6%9c%9f)
- [代码演示](#%e4%bb%a3%e7%a0%81%e6%bc%94%e7%a4%ba)
  - [项目](#%e9%a1%b9%e7%9b%ae)
  - [示例服务类](#%e7%a4%ba%e4%be%8b%e6%9c%8d%e5%8a%a1%e7%b1%bb)
  - [验证不同生命周期的实现](#%e9%aa%8c%e8%af%81%e4%b8%8d%e5%90%8c%e7%94%9f%e5%91%bd%e5%91%a8%e6%9c%9f%e7%9a%84%e5%ae%9e%e7%8e%b0)
    - [注册不同生命周期的服务](#%e6%b3%a8%e5%86%8c%e4%b8%8d%e5%90%8c%e7%94%9f%e5%91%bd%e5%91%a8%e6%9c%9f%e7%9a%84%e6%9c%8d%e5%8a%a1)
    - [在方法参数中获得服务进行验证](#%e5%9c%a8%e6%96%b9%e6%b3%95%e5%8f%82%e6%95%b0%e4%b8%ad%e8%8e%b7%e5%be%97%e6%9c%8d%e5%8a%a1%e8%bf%9b%e8%a1%8c%e9%aa%8c%e8%af%81)
  - [其他方式注册服务(以单例模式为例)](#%e5%85%b6%e4%bb%96%e6%96%b9%e5%bc%8f%e6%b3%a8%e5%86%8c%e6%9c%8d%e5%8a%a1%e4%bb%a5%e5%8d%95%e4%be%8b%e6%a8%a1%e5%bc%8f%e4%b8%ba%e4%be%8b)
    - [直接new对象](#%e7%9b%b4%e6%8e%a5new%e5%af%b9%e8%b1%a1)
    - [通过工厂模式注册对象](#%e9%80%9a%e8%bf%87%e5%b7%a5%e5%8e%82%e6%a8%a1%e5%bc%8f%e6%b3%a8%e5%86%8c%e5%af%b9%e8%b1%a1)
  - [尝试注册服务](#%e5%b0%9d%e8%af%95%e6%b3%a8%e5%86%8c%e6%9c%8d%e5%8a%a1)
  - [移除和替换服务](#%e7%a7%bb%e9%99%a4%e5%92%8c%e6%9b%bf%e6%8d%a2%e6%9c%8d%e5%8a%a1)
    - [移除服务](#%e7%a7%bb%e9%99%a4%e6%9c%8d%e5%8a%a1)
    - [替换服务](#%e6%9b%bf%e6%8d%a2%e6%9c%8d%e5%8a%a1)
  - [泛型服务注册](#%e6%b3%9b%e5%9e%8b%e6%9c%8d%e5%8a%a1%e6%b3%a8%e5%86%8c)
- [服务对象的获取](#%e6%9c%8d%e5%8a%a1%e5%af%b9%e8%b1%a1%e7%9a%84%e8%8e%b7%e5%8f%96)

# 为什么要使用依赖注入框架(IoC框架)
* 借助依赖注入框架，我们可以轻松管理类之间的依赖，帮助我们在构建应用是遵循设计规则，确保代码的可维护性和可拓展性
* ASP.NET Core的整个架构中，依赖注入框架提供了对象创建和生命周期管理的核心能力，各个组件相互写作，也是由依赖注入框架的能力来实现的

## 依赖注入框架组件包
```
Microsoft.Extensions.DependencyInjection.Abstractions //抽象包
Microsoft.Extensions.DependencyInjection  //具体实现
```
## 依赖注入框架核心类型
```
IServiceCollection //负责服务的注册
ServiceDescriptor //每个服务注册时的信息
IServiceProvider  //具体的容器，也是由ServiceCollection Build出来
IServiceScope  //表示一个容器的子容器的生命周期
```
## 生命周期
```
Singleton  //单例：在整个根容器的生命周期内获得的都是同一个单例对象
Scoped     //作用域：在我的Scope的生命周期内，如果我的容器释放掉，则意味着我的对象释放掉，在这个生命周期范围内获得到的是一个单例对象
Transient  //瞬时：每一次从容器里获取对象时都产生一个新的对象
```

# 代码演示
## 项目
创建名为`DependencyInjectionDemo`的`ASP.NET Core`项目，类型为`API`
## 示例服务类
一共有5个示例服务类接口，分别为
```
IGenericService<T>
    |-- GenericService<T>   //对应实现类
IMyScopeService
    |-- MyScopeService      //对应实现类
IMySingletonService
    |-- MySingletonService  //对应实现类
IMyTransientService
    |-- MyTransientService  //对应实现类
IOrderService
    |-- OrderService        //对应实现类
    |-- OrderServiceEX      //对应实现类
```
因为是示例服务类，所以所有类和服务均没有属性和方法，纯粹为了验证服务注册和服务对象

## 验证不同生命周期的实现
### 注册不同生命周期的服务
在`Startup.ConfigureServices`方法中新增以下代码
``` csharp
// 注册Singleton服务
services.AddSingleton<IMySingletonService, MySingletonService>();
// 注册Scope服务
services.AddScoped<IMyScopeService, MyScopeService>();
// 注册Transient服务
services.AddTransient<IMyTransientService, MyTransientService>();
```
### 在方法参数中获得服务进行验证
1. 修改`WeatherForecastController`类的`Route`标识为`[Route("[controller]/[action]")]`，方便进行测试
2. 在`WeatherForecastController`新增`GetService`方法，代码如下：
   ``` csharp
    [HttpGet]
    public int GetService([FromServices] IMySingletonService singletonService1,
        [FromServices] IMySingletonService singletonService2,
        [FromServices] IMyScopeService scopeService1,
        [FromServices] IMyScopeService scopeService2,
        [FromServices] IMyTransientService transientService1,
        [FromServices] IMyTransientService transientService2)
    {
        Console.WriteLine($"{nameof(singletonService1)}:{singletonService1.GetHashCode()}");
        Console.WriteLine($"{nameof(singletonService2)}:{singletonService2.GetHashCode()}");
        Console.WriteLine($"{nameof(scopeService1)}:{scopeService1.GetHashCode()}");
        Console.WriteLine($"{nameof(scopeService2)}:{scopeService2.GetHashCode()}");
        Console.WriteLine($"{nameof(transientService1)}:{transientService1.GetHashCode()}");
        Console.WriteLine($"{nameof(transientService2)}:{transientService2.GetHashCode()}");
        Console.WriteLine("=========请求结束========");
        return 1;
    }
   ```
3. 运行项目，访问`/WeatherForecast/GetService`，控制台会打印出类似以下信息
   ```
    singletonService1:23488915
    singletonService2:23488915
    scopeService1:24854661
    scopeService2:24854661
    transientService1:38972574
    transientService2:14645893
    =========请求结束========
   ```
   可以看出`IMySingletonService`的实现对象属于同一个对象，`IMyTransientService`的实现对象有多个，目前看`IMyScopeService`的实现对象为同一个，但是再次访问`/WeatherForecast/GetService`接口，就可以发现`IMyScopeService`的实现对象为新的对象，而`IMySingletonService`的实现对象还是上次访问的那个，打印信息如下
   ```
    singletonService1:23488915
    singletonService2:23488915
    scopeService1:6630602
    scopeService2:6630602
    transientService1:5024928
    transientService2:38414640
    =========请求结束========
   ```

## 其他方式注册服务(以单例模式为例)
### 直接new对象
在`Startup.ConfigureServices`方法中新增以下代码
``` csharp
services.AddSingleton<IOrderService>(new OrderService());
```
### 通过工厂模式注册对象
使用工厂模式注册对象，可以在委托中使用IServiceProvider参数，这也就意味着可以从容器里获取多个对象，然后进行组装，得到我们最终需要的实现实例，可以把工厂类设计的比较复杂，比如说我们的实现类依赖了容器里面的另外一个类的情况，或者我们期望用另外一个类来包装我们原有的实现的时候
在`Startup.ConfigureServices`方法中新增以下代码
``` csharp
services.AddSingleton<IOrderService>(serviceProvider =>
{
    return new OrderServiceEx();
});
```

## 尝试注册服务
尝试注册服务有两种情况
1. 当指定接口已有实现类，则不再注册服务，代码如下：
   ``` csharp
   services.TryAddSingleton<IOrderService, OrderServiceEx>();
   ```
2. 当指定接口已有实现类，但是已注册的实现类不包含当前指定的实现类，则注册进去，如果已经包含当前的实现类，则不再注册服务，代码如下：
   ``` csharp
   services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService,OrderService>());
   ```
   在控制器里验证是否有多个实现类注册，可通过下面的方法验证(需要自行注释或修改Startup里面的服务注册情况)
   ``` csharp
    [HttpGet]
    public int GetServiceList([FromServices] IEnumerable<IOrderService> orderServices)
    {
        foreach (var item in orderServices)
        {
            Console.WriteLine($"获取到服务实例：{item.ToString()}:{item.GetHashCode()}");
        }
        return 1;
    }
   ```
## 移除和替换服务
### 移除服务
指的是直接从容器中移除指定接口的所有实现类，代码如下：
``` csharp
services.RemoveAll<IOrderService>();
```
### 替换服务
指的是替换指定接口的实现类，同时也会替换该服务的生命周期，代码如下：
``` csharp
services.Replace(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>());
```

## 泛型服务注册
即对泛型服务注册，代码如下：
``` csharp
services.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));
```
可以通过在控制器的构造函数中获取到服务对象，代码如下：
``` csharp
public WeatherForecastController(ILogger<WeatherForecastController> logger,IOrderService orderService,IGenericService<IOrderService> genericService)
{
    _logger = logger;
}
```
可以通过断点查看最终`IGenericService`的`IOrderService`为哪个实现类

# 服务对象的获取
通过上面可以看出，服务对象有两种获取方式，一种是通过构造函数直接注入，一种是通过函数参数，使用`[FromServices]`标签来注入

**一般按照使用情况来确定用哪种方式，如果整个类使用地方比较多，则使用构造函数注入，如果只有某一个方法使用，则一般使用函数参数来注入**