---
title: .NET Core开发实战课程备忘(5) -- 配置框架：让服务无缝适应各种环境
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

- [概念](#%e6%a6%82%e5%bf%b5)
  - [配置框架的核心包](#%e9%85%8d%e7%bd%ae%e6%a1%86%e6%9e%b6%e7%9a%84%e6%a0%b8%e5%bf%83%e5%8c%85)
  - [框架配置](#%e6%a1%86%e6%9e%b6%e9%85%8d%e7%bd%ae)
  - [配置框架核心类型](#%e9%85%8d%e7%bd%ae%e6%a1%86%e6%9e%b6%e6%a0%b8%e5%bf%83%e7%b1%bb%e5%9e%8b)
  - [配置框架扩展点](#%e9%85%8d%e7%bd%ae%e6%a1%86%e6%9e%b6%e6%89%a9%e5%b1%95%e7%82%b9)
- [代码实现](#%e4%bb%a3%e7%a0%81%e5%ae%9e%e7%8e%b0)
  - [创建项目](#%e5%88%9b%e5%bb%ba%e9%a1%b9%e7%9b%ae)
  - [完善代码](#%e5%ae%8c%e5%96%84%e4%bb%a3%e7%a0%81)


# 概念
## 配置框架的核心包
与依赖注入框架的核心包类似，使用的是接口实现分离的设计模式
```
Microsoft.Extensions.Configuration              //实现包
Microsoft.Extensions.Configuration.Abstractions //抽象包
```
## 框架配置
* 以`key-value`字符串键值对的方式抽象了配置
* 支持从各种不同的数据源读取配置，比如从命令行读取、环境变量读取、从文件读取等

## 配置框架核心类型
```
IConfiguration
IConfigurationRoot
IConfigurationSection
IConfigurationBuilder
```
## 配置框架扩展点
可以通过拓展点注入我们自己的配置源，也就是我们可以指定任意我们指定的配置源到我们的配置框架中去
```
IConfigurationSource
IConfigurationProvider
```

# 代码实现
## 创建项目
创建名字为`ConfigurationDemo`的`控制台应用`，通过`nuget`引入以下两个包：
```
Microsoft.Extensions.Configuration
Microsoft.Extensions.Configuration.Abstractions
```
## 完善代码
在`Program.cs`的`Main`完善测试代码，具体代码如下：
``` csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
namespace ConfigurationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"sectionA:key4", "value4"}
            });
            var configurationRoot = builder.Build();
            Console.WriteLine($"key1={configurationRoot["key1"]}");
            Console.WriteLine($"key2={configurationRoot["key2"]}");
            var sectionA = configurationRoot.GetSection("sectionA");
            Console.WriteLine($"sectionA:key4={sectionA["key4"]}");
        }
    }
}
```
创建一个`ConfigurationBuilder`对象，添加内存配置源，然后调用`Build`方法来生成一个`IConfigurationRoot`对象，通过这个对象可以直接获取配置源的配置项，用`:`来区分多个层级，通过`GetSetion`可获取指定层级，可以依次推各个层级的键值对