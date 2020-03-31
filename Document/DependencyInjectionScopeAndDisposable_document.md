---
title: .NET Core开发实战课程备忘(3) -- 作用域与对象释放行为
date: 2020/03/31
tag:
- ASP.NET Core
- 教程备忘
categories:
- .NET Core开发实战课程备忘
keywords:
- .NET Core
- ASP.NET Core
---

- [实现`IDisposable`接口类型的释放](#%e5%ae%9e%e7%8e%b0idisposable%e6%8e%a5%e5%8f%a3%e7%b1%bb%e5%9e%8b%e7%9a%84%e9%87%8a%e6%94%be)
- [建议](#%e5%bb%ba%e8%ae%ae)
- [代码验证](#%e4%bb%a3%e7%a0%81%e9%aa%8c%e8%af%81)
  - [项目](#%e9%a1%b9%e7%9b%ae)
  - [创建测试服务](#%e5%88%9b%e5%bb%ba%e6%b5%8b%e8%af%95%e6%9c%8d%e5%8a%a1)
  - [测试Transient服务的释放时机](#%e6%b5%8b%e8%af%95transient%e6%9c%8d%e5%8a%a1%e7%9a%84%e9%87%8a%e6%94%be%e6%97%b6%e6%9c%ba)
  - [测试Scope服务的释放时机](#%e6%b5%8b%e8%af%95scope%e6%9c%8d%e5%8a%a1%e7%9a%84%e9%87%8a%e6%94%be%e6%97%b6%e6%9c%ba)
  - [测试Singleton服务的释放时机](#%e6%b5%8b%e8%af%95singleton%e6%9c%8d%e5%8a%a1%e7%9a%84%e9%87%8a%e6%94%be%e6%97%b6%e6%9c%ba)
- [避坑](#%e9%81%bf%e5%9d%91)
  - [自己new服务](#%e8%87%aa%e5%b7%b1new%e6%9c%8d%e5%8a%a1)
  - [在跟容器获取Transient服务](#%e5%9c%a8%e8%b7%9f%e5%ae%b9%e5%99%a8%e8%8e%b7%e5%8f%96transient%e6%9c%8d%e5%8a%a1)

# 实现`IDisposable`接口类型的释放
* `DI`只负责释放其创建的对象实例
* `DI`在容器或子容器释放时，释放尤其创建的对象实例

# 建议
* 避免在根容器获取实现了`IDisposable`接口的瞬时服务
* 避免手动创建实现了`IDisposable`对象，应该使用容器来管理其生命周期

# 代码验证
## 项目
创建名为`DependencyInjectionScopeAndDisposableDemo`的`ASP.NET Core`项目，类型为`API`
## 创建测试服务
创建测试服务类`OrderService.cs`，代码如下：
``` csharp
using System;
namespace DependencyInjectionScopeAndDisposableDemo.Services
{
    public interface IOrderService { }
    public class DisposableOrderService : IOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"DisposableOrderService Disposed:{this.GetHashCode()}");
        }
    }
}
```
## 测试Transient服务的释放时机
在`Startup.ConfigureServices`里注册一个瞬时服务
``` csharp
services.AddTransient<IOrderService, DisposableOrderService>();
```
在`WeatherForecastController`里的`Get`方法通过方法参数的形式获取两个`IOrderService`接口对象，代码如下：
``` csharp
[HttpGet]
public int Get([FromServices] IOrderService orderService1, [FromServices] IOrderService orderService2)
{
    Console.WriteLine("=====接口请求处理结束====");
    return 1;
}
```
保存之后运行项目，可以看到在打印接口请求处理结束之后两个对象都被释放掉，类似以下信息
```
=====接口请求处理结束====
DisposableOrderService Disposed:64923656
DisposableOrderService Disposed:11404313
```
得出的结论是：**transient**对象会在使用后被释放
## 测试Scope服务的释放时机
注释掉上一步注册瞬时服务的代码，重新注册一个`scope`服务，这里使用工厂模式，只是熟悉以下工厂模式的写法，没特殊意义
``` csharp
services.AddScoped<IOrderService>(serviceProvider => new DisposableOrderService());
```
这时运行项目，控制台会打印出一个对象被释放，因为`scope`服务在当前容器内为单例，下面就这个结论再次测试以下，用using创建一个服务容器出来，看是否能创建出新的服务对象，并且代码运行超过using范围，服务对象是否会被释放。在`WeatherForecastController`的`Get`方法新增创建容器和获取服务操作，代码如下：
``` csharp
[HttpGet]
public int Get([FromServices] IOrderService orderService1, [FromServices] IOrderService orderService2)
{
    Console.WriteLine("========1==========");
    using (var scope = HttpContext.RequestServices.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<IOrderService>();
    }
    Console.WriteLine("========2==========");

    Console.WriteLine("=====接口请求处理结束====");
    return 1;
}
```
`HttpContext.RequestServices.CreateScope()`表示从根容器中创建一个子容器出来，`scope.ServiceProvider.GetService<IOrderService>()`表示从创建出来的子容器获取`IOrderService`的服务实现对象，运行项目可以得到两个对象被释放的信息，类似以下信息：
```
========1==========
DisposableOrderService Disposed:5568949
========2==========
=====接口请求处理结束====
DisposableOrderService Disposed:31307802
```
得出结论是：**scope**对象会在对象产生的容器被释放的时候同时一起释放

## 测试Singleton服务的释放时机
注释掉上一步注册scope服务的代码，重新注册一个`singleton`服务，这里一样使用工厂模式
``` csharp
services.AddSingleton<IOrderService>(serviceProvider => new DisposableOrderService());
```
将`WeatherForecastController`的`Get`方法修改为以下代码：
``` csharp
[HttpGet]
public int Get([FromServices] IOrderService orderService1, [FromServices] IOrderService orderService2,[FromServices] IHostApplicationLifetime hostApplicationLifetime,[FromQuery]bool isStop=false)
{
    if (isStop)
    {
        hostApplicationLifetime.StopApplication();
    }

    Console.WriteLine("=====接口请求处理结束====");
    return 1;
}
```
这里有获取了另一个服务`IHostApplicationLifetime`，这个服务对象控制了整个应用程序的生命周期，可以调用这个服务对象的`StopApplication`方法来停止应用程序，即停止整个站点，为了方便测试这里用了`isStop`这个参数来控制是否停止应用程序

运行项目，不管怎么刷新`/WeatherForecast`链接，都不会有对象被释放的信息打印出来，访问`/WeatherForecast?isStop=true`的时候，会看到应用程序被停止，同时打印出对象被释放的信息，类似以下信息：
```
Application is shutting down...
DisposableOrderService Disposed:3165221
```
得出结论是：**scope对象只会在根容器释放的时候才会被释放，即所有单例模式的对象都会被注册在根容器上面**

# 避坑
## 自己new服务
开始有个结论：`DI`只负责释放其创建的对象实例，这里进行验证以下自己new一个对象然后注入到容器中，看看是否能释放。这里注入了一个`singleton`对象，需注释掉上面做测试的代码，代码如下
```
var myOrderService = new DisposableOrderService();
services.AddSingleton<IOrderService>(myOrderService);
```
控制器代码不用改，运行项目，访问`/WeatherForecast`发现不会有对象被释放，访问`/WeatherForecast?isStop=true`时，应用程序被停止，但是也一样没有对象被释放的信息，所以这个对象最终还是没被释放

## 在跟容器获取Transient服务
注释掉上面的测试代码，在`Startup.ConfigureServices`中注册一个瞬时服务，代码如下：
``` csharp
services.AddTransient<IOrderService, DisposableOrderService>();
```
在`Startup.Configure`中从根容器获取瞬时服务对象，代码如下：
``` csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
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
```
将`WeatherForecastController`的`Get`方法修改为以下代码：
``` csharp
[HttpGet]
public int Get([FromServices] IHostApplicationLifetime hostApplicationLifetime, [FromQuery]bool isStop = false)
{
    if (isStop)
    {
        hostApplicationLifetime.StopApplication();
    }

    Console.WriteLine("=====接口请求处理结束====");
    return 1;
}
```
运行项目，访问`/WeatherForecast`接口，发现并不会有对象被释放的信息，在带上`isStop=true`的参数的时候，应用程序被停止，这时才有对象被释放的信息，类似信息如下：
```
Application is shutting down...
DisposableOrderService Disposed:41149443
```
得出结论：**由于根容器只会在应用程序整个退出时回收，这就意味着即使这是个瞬时服务，但是应用程序不退出，这些对象会一直积累在应用程序内不得释放**