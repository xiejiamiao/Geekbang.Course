---
title: .NET Core开发实战课程备忘(7) -- 环境变量配置提供程序：容器环境下配置注入的最佳途径
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

# 使用场景
* 在`Docker`中运行
* 在`Kubernetes`中运行
* 需要设置`ASP.NET Core`的一些内置特殊配置时

# 特性
* 对应配置的分层键，支持用双下划线"`--`"代替"`:`"
* 支持根据前缀加载


# 代码示例
## 创建项目
创建名字为`ConfigurationEnvironmentVariablesDemo`的`控制台应用`，通过`nuget`引入以下三个包：
```
Microsoft.Extensions.Configuration
Microsoft.Extensions.Configuration.Abstractions
Microsoft.Extensions.Configuration.EnvironmentVariables
```
## 获取环境变量配置
修改`Program.Main`方法，代码如下：
``` csharp
using System;
using Microsoft.Extensions.Configuration;

namespace ConfigurationEnvironmentVariablesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();
            var configurationRoot = builder.Build();
            Console.WriteLine($"KEY3={configurationRoot["KEY3"]}");
            var section = configurationRoot.GetSection("SECTIONA");
            Console.WriteLine($"SECTIONA:KEY2={section["KEY2"]}");
        }
    }
}
```
配置测试参数：
* 在`Visual Studio 2019`中，可以右键项目名称->属性->调试->环境变量中添加以下内容：
  ```
    "DIMSUM_KEY1": "dimsum_value1",
    "KEY3": "value3",
    "KEY1": "value1",
    "SECTIONA__KEY2": "value2",
    "KEY4": "value4"
  ```
* 在`Visual Studio Code`中，可以编辑`launchSettings.json`文件，新增`environmentVariables`字段，具体代码如下:
  ``` json
    {
        "profiles": {
            "ConfigurationEnvironmentVariablesDemo": {
            "commandName": "Project",
            "environmentVariables": {
                "DIMSUM_KEY1": "dimsum_value1",
                "KEY3": "value3",
                "KEY1": "value1",
                "SECTIONA__KEY2": "value2",
                "KEY4": "value4"
            }
            }
        }
    }
  ```
运行项目，会得到以下信息：
```
KEY3=value3
SECTIONA:KEY2=value2
```

## 前缀过滤
过滤指定前缀的环境变量，具体代码如下：
``` csharp
using System;
using Microsoft.Extensions.Configuration;

namespace ConfigurationEnvironmentVariablesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables("DIMSUM_");
            var configurationRoot = builder.Build();
            Console.WriteLine($"DIMSUM_KEY1={configurationRoot["KEY1"]}");
        }
    }
}

```
运行项目可以得到以下信息
```
DIMSUM_KEY1=dimsum_value1
```
说明这里获取到的是`DIMSUM_KEY1`这个配置，而非`KEY`这个配置