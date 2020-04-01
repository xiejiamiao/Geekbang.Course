---
title: .NET Core开发实战课程备忘(9) -- 自定义配置数据源：低成本实现定制配置方案
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

- [拓展步骤](#%e6%8b%93%e5%b1%95%e6%ad%a5%e9%aa%a4)
- [代码实现](#%e4%bb%a3%e7%a0%81%e5%ae%9e%e7%8e%b0)
  - [创建项目](#%e5%88%9b%e5%bb%ba%e9%a1%b9%e7%9b%ae)
  - [创建自定义数据源`Provider`](#%e5%88%9b%e5%bb%ba%e8%87%aa%e5%ae%9a%e4%b9%89%e6%95%b0%e6%8d%ae%e6%ba%90provider)
  - [创建自定义数据源`Source`](#%e5%88%9b%e5%bb%ba%e8%87%aa%e5%ae%9a%e4%b9%89%e6%95%b0%e6%8d%ae%e6%ba%90source)
  - [创建`IConfigurationBuilder`扩展方法](#%e5%88%9b%e5%bb%baiconfigurationbuilder%e6%89%a9%e5%b1%95%e6%96%b9%e6%b3%95)
  - [引用自定义配置源并监听配置源变化](#%e5%bc%95%e7%94%a8%e8%87%aa%e5%ae%9a%e4%b9%89%e9%85%8d%e7%bd%ae%e6%ba%90%e5%b9%b6%e7%9b%91%e5%90%ac%e9%85%8d%e7%bd%ae%e6%ba%90%e5%8f%98%e5%8c%96)

# 拓展步骤
* 实现`IConfigurationSource`
* 实现`IConfigurationProvider`
* 实现`AddXXX`扩展方法

# 代码实现
## 创建项目
创建名字为`ConfigurationCustomDemo`的`控制台应用`，通过`nuget`引入以下两个包：
```
Microsoft.Extensions.Configuration
Microsoft.Extensions.Configuration.Abstractions
```
## 创建自定义数据源`Provider`
创建`MyConfigurationProvider`，继承`ConfigurationProvider`这个抽象类，调用基类的`OnReload`方法能触发重新加载配置，这里因为要自动修改和触发配置，所以这个类里用了`Timer`对象来模拟配置变化的操作，具体代码如下：
``` csharp
using System;
using System.Timers;
using Microsoft.Extensions.Configuration;

namespace ConfigurationCustomDemo
{
    internal class MyConfigurationProvider:ConfigurationProvider
    {
        private Timer timer;

        public MyConfigurationProvider():base()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Load(true);
        }

        void Load(bool reload)
        {
            this.Data["lastTime"] = DateTime.Now.ToString();
            if (reload)
            {
                base.OnReload();
            }
        }
    }
}
```
## 创建自定义数据源`Source`
创建`MyConfigurationSource`，实现`IConfigurationSource`接口，在`Build`方法直接返回上面创建的`Provider`，具体代码如下：
``` csharp
using Microsoft.Extensions.Configuration;

namespace ConfigurationCustomDemo
{
    internal class MyConfigurationSource:IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MyConfigurationProvider();
        }
    }
}
```
## 创建`IConfigurationBuilder`扩展方法
可以看到上面两个类的都是用`internal`的访问修饰符，这是因为一般在`Provider`和`Source`都是通过拓展方法来调用，而不会将自己直接暴露被调用者，所以接下来要创建一个`IConfigurationBuilder`的扩展方法，代码如下：
``` csharp
using Microsoft.Extensions.Configuration;

namespace ConfigurationCustomDemo
{
    public static class MyConfigurationBuilderExtension
    {
        public static IConfigurationBuilder AddMyConfiguration(this IConfigurationBuilder builder)
        {
            builder.Add(new MyConfigurationSource());
            return builder;
        }
    }
}
```

## 引用自定义配置源并监听配置源变化
`Program.Main`方法修改如下：
``` csharp
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationCustomDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddMyConfiguration();
            var configurationRoot = builder.Build();

            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                var lastTime = configurationRoot["lastTime"];
                Console.WriteLine($"lastTime={lastTime}");
                Console.WriteLine("======");
            });

            Console.ReadKey();
        }
    }
}
```
运行代码，可以看到控制台3秒钟打印一次当前时间的效果