---
title: .NET Core开发实战课程备忘(3) -- 用Autofac增强容器能力
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

- [引入Autofac增强什么能力](#%e5%bc%95%e5%85%a5autofac%e5%a2%9e%e5%bc%ba%e4%bb%80%e4%b9%88%e8%83%bd%e5%8a%9b)
- [核心扩展点](#%e6%a0%b8%e5%bf%83%e6%89%a9%e5%b1%95%e7%82%b9)
- [集成Autofac](#%e9%9b%86%e6%88%90autofac)
- [代码验证](#%e4%bb%a3%e7%a0%81%e9%aa%8c%e8%af%81)
  - [项目与依赖](#%e9%a1%b9%e7%9b%ae%e4%b8%8e%e4%be%9d%e8%b5%96)
  - [在代码中引入`Autofac`](#%e5%9c%a8%e4%bb%a3%e7%a0%81%e4%b8%ad%e5%bc%95%e5%85%a5autofac)
  - [创建测试服务](#%e5%88%9b%e5%bb%ba%e6%b5%8b%e8%af%95%e6%9c%8d%e5%8a%a1)
  - [获取`Autofac`根容器](#%e8%8e%b7%e5%8f%96autofac%e6%a0%b9%e5%ae%b9%e5%99%a8)
  - [一般注册服务](#%e4%b8%80%e8%88%ac%e6%b3%a8%e5%86%8c%e6%9c%8d%e5%8a%a1)
  - [基于名字注册服务](#%e5%9f%ba%e4%ba%8e%e5%90%8d%e5%ad%97%e6%b3%a8%e5%86%8c%e6%9c%8d%e5%8a%a1)
  - [属性注入](#%e5%b1%9e%e6%80%a7%e6%b3%a8%e5%85%a5)
  - [AOP切面拦截器](#aop%e5%88%87%e9%9d%a2%e6%8b%a6%e6%88%aa%e5%99%a8)
  - [创建子容器](#%e5%88%9b%e5%bb%ba%e5%ad%90%e5%ae%b9%e5%99%a8)

# 引入Autofac增强什么能力
* 基于名称的注入：需要把一个服务按照名称来区分它的不同实现
* 属性注入：直接把服务注册到某个类的属性里面去，而不需要定义构造函数
* 子容器：类似原生的scope，但是功能更加丰富
* 基于动态代理的AOP：当我们需要在服务中注入我们额外的行为的时候

# 核心扩展点
`IServiceProviderFactory<TContainerBuilder>`：第三方的依赖注入容器都是使用这个类来作为拓展点，把自己注入到整个框架里面来，也就是我们在使用依赖注入框架的时候，不需要关注谁家的特性谁家接口时怎么样的，我们直接使用官方核心的定义即可，不需要直接依赖这些框架
# 集成Autofac
* `Autofac.Extensions.DependencyInjection`
* `Autofac.Extras.DynamicProxy`

# 代码验证
## 项目与依赖
创建名字为`DependencyInjectionAutofacDemo`的`ASP.NET Core`项目，类型为`API`

通过`nuget`引入以下两个包
```
Autofac.Extensions.DependencyInjection
Autofac.Extras.DynamicProxy
```
## 在代码中引入`Autofac`
在`Program.cs`的`CreateDefaultBuilder`后面添加以下代码
```
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
```
`UseServiceProviderFactory`用来注册第三方容器的入口

在`Startup`中新增`ConfigureContainer`方法，代码如下：
``` csharp
public void ConfigureContainer(ContainerBuilder builder)
{
}
```
至此`Autofac`框架引入完毕，下面要创建测试服务类
## 创建测试服务
创建测试服务`MyService.cs`类，具体代码如下：
``` csharp
using System;
namespace DependencyInjectionAutofacDemo.Services
{
    public interface IMyService
    {
        void ShowCode();
    }
    public class MyService : IMyService
    {
        public void ShowCode()
        {
            Console.WriteLine($"MyService.ShowCode:{GetHashCode()}");
        }
    }
    public class MyServiceV2 : IMyService
    {
        public MyNameService MyNameService { get; set; }
        public void ShowCode()
        {
            Console.WriteLine($"MyServiceV2.ShowCode:{GetHashCode()},MyNameService是否为空:{MyNameService==null}");
        }
    }
    public class MyNameService
    {
    }
}
```
创建测试拦截器`MyInterceptor.cs`，代码如下：
``` csharp
using System;
using Castle.DynamicProxy;

namespace DependencyInjectionAutofacDemo.Services
{
    public class MyInterceptor:IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Intercept before,Method:{invocation.Method.Name}");
            invocation.Proceed();
            Console.WriteLine($"Intercept after,Method:{invocation.Method.Name}");
        }
    }
}
```
* IInterceptor 是Autofac面向切面最重要的一个接口，他可以把我们的逻辑注入到方法的切面里面去
* `invocation.Proceed()`是指具体方法的执行，如果这句不执行，就相当于把切面方法拦截了，让具体类的方法不执行

## 获取`Autofac`根容器
在`Startup`里新增类型为`ILifetimeScope`的`AutofacContainer`属性，然后在`Configure`方法中为这个属性复制为`Autofac`的根容器，具体代码如下：
``` csharp
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using DependencyInjectionAutofacDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionAutofacDemo
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
            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
        }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
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
```

## 一般注册服务
在`ConfigureContainer`方法中进行服务注册，然后在`Configure`方法中获取服务实现对象，调用服务的`ShowCode`方法，具体代码如下:
``` csharp
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.RegisterType<MyService>().As<IMyService>();
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    var serviceNoName = this.AutofacContainer.Resolve<IMyService>();
    serviceNoName.ShowCode();
}
```
**Autofac注册服务与`ASP.NET Core`写法相反，先注册实现类，然后再标记这个实现类为哪种类型**

运行项目会看到控制台打印了`MyService`对象调用`ShowCode`方法时候打印的信息，类似信息如下：
```
MyService.ShowCode:16336406
```

## 基于名字注册服务
注释掉上一步的测试代码，一样是在`ConfigureContainer`方法中进行服务注册，然后在`Configure`方法中获取服务实现对象，调用服务的`ShowCode`方法，具体代码如下:
``` csharp
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.RegisterType<MyServiceV2>().Named<IMyService>("service2");
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    var service = this.AutofacContainer.ResolveNamed<IMyService>("service2");
    service.ShowCode();
}
```
运行项目会看到控制台打印了`MyServiceV2`对象调用`ShowCode`方法时候打印的信息，类似信息如下：
```
MyServiceV2.ShowCode:16336406,MyNameService是否为空:True
```

## 属性注入
注释掉上一步的测试代码，在`ConfigureContainer`方法中进行服务注册，注意需要先将属性的服务先进行注册，再进行调用方的服务注册，然后一样再`Configure`中获取对象，调用`ShowCode`方法
``` csharp
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.RegisterType<MyNameService>();
    builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired();
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    var service = this.AutofacContainer.Resolve<IMyService>();
    service.ShowCode();
}
```
运行项目会看到控制台打印了`MyServiceV2`对象调用`ShowCode`方法时候打印的信息，类似信息如下：
```
MyServiceV2.ShowCode:10309404,MyNameService是否为空:False
```
可以发现`MyNameService`属性已经不为空了，通过属性注入的操作注入到了服务对象中去，打断点进行调试，可以看出`MyNameService`类型就是上面注册的类型

## AOP切面拦截器
注释掉上一步的测试代码，先在`ConfigureContainer`方法中注册拦截器，然后在服务，并指定拦截器为刚刚所注册的拦截器，并且允许接口拦截器生效，获取服务与上一步操作一致
``` csharp
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.RegisterType<MyInterceptor>();
    builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableInterfaceInterceptors();
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    var service = this.AutofacContainer.Resolve<IMyService>();
    service.ShowCode();
}
```
运行项目，可以看到控制台在打印出`MyServiceV2`的`ShowCode`方法所打印的信息前后，有拦截器打印出来的信息，类似信息如下：
```
Intercept before,Method:ShowCode
MyServiceV2.ShowCode:25116876,MyNameService是否为空:True
Intercept after,Method:ShowCode
```

## 创建子容器
子容器主要适用于将服务注册进指定名字的容器里，这样只有在创建出指定名字的容器才可获取到服务对象，其他容器无法获得该服务对象，具体代码如下：
``` csharp
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.RegisterType<MyNameService>().InstancePerMatchingLifetimeScope("myScope");
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    using (var myScope = this.AutofacContainer.BeginLifetimeScope("myScope"))
    {
        var service0 = myScope.Resolve<MyNameService>();
        using (var scope = myScope.BeginLifetimeScope())
        {
            var service1 = scope.Resolve<MyNameService>();
            var service2 = scope.Resolve<MyNameService>();

            Console.WriteLine($"service0=service1:{service0==service1}");
            Console.WriteLine($"service1=service2:{service1==service2}");
        }
    }
}
```
运行代码可看到对象获取成功，并且获取到的对象在作用域内为同一个对象，类似信息如下：
```
service0=service1:True
service1=service2:True
```
如果这时候不通过创建指定名字的容器来获得服务对象，会发现代码运行直接报错