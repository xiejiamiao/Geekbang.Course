---
title: .NET Core开发实战课程备忘(6) -- 命令行配置提供程序：最简单快捷的配置注入方法
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

- [命令行参数支持的格式](#%e5%91%bd%e4%bb%a4%e8%a1%8c%e5%8f%82%e6%95%b0%e6%94%af%e6%8c%81%e7%9a%84%e6%a0%bc%e5%bc%8f)
- [命令替换模式](#%e5%91%bd%e4%bb%a4%e6%9b%bf%e6%8d%a2%e6%a8%a1%e5%bc%8f)
- [代码示例](#%e4%bb%a3%e7%a0%81%e7%a4%ba%e4%be%8b)
  - [创建项目](#%e5%88%9b%e5%bb%ba%e9%a1%b9%e7%9b%ae)
  - [测试支持命令行参数的三种格式](#%e6%b5%8b%e8%af%95%e6%94%af%e6%8c%81%e5%91%bd%e4%bb%a4%e8%a1%8c%e5%8f%82%e6%95%b0%e7%9a%84%e4%b8%89%e7%a7%8d%e6%a0%bc%e5%bc%8f)
  - [测试命令替换](#%e6%b5%8b%e8%af%95%e5%91%bd%e4%bb%a4%e6%9b%bf%e6%8d%a2)

# 命令行参数支持的格式
* 无前缀的`key=value`模式
* 双中横线`--key=value`或`--key value`模式
* 正斜杠`/key=value`或`/key value`模式
*备注：等号分隔符和空格分隔符不能混用*

# 命令替换模式
* 必须以单横线`-`或双横线`--`开头
* 映射字典不能包含重复key
* 主要作用是命令缩写的作用

# 代码示例
## 创建项目
创建名字为`ConfigurationCommandLineDemo`的`控制台应用`，通过`nuget`引入以下三个包：
```
Microsoft.Extensions.Configuration
Microsoft.Extensions.Configuration.Abstractions
Microsoft.Extensions.Configuration.CommandLine
```
## 测试支持命令行参数的三种格式
修改`Program.Main`方法，代码如下：
``` csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ConfigurationCommandLineDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var configurationRoot = builder.Build();
            Console.WriteLine($"CommandLineKey1:{configurationRoot["CommandLineKey1"]}");
            Console.WriteLine($"CommandLineKey2:{configurationRoot["CommandLineKey2"]}");
            Console.WriteLine($"CommandLineKey3:{configurationRoot["CommandLineKey3"]}");
            Console.ReadKey();
        }
    }
}

```
配置测试参数：
* 在`Visual Studio 2019`中，可以右键项目名称->属性->调试->应用程序参数中输入以下内容：
  ```
  CommandLineKey1=value1 --CommandLineKey2=value2 /CommandLineKey3=value3
  ```
* 在`Visual Studio Code`中，可以编辑`launchSettings.json`文件，新增`commandLineArgs`字段，具体代码如下:
  ``` json
    {
        "profiles": {
            "ConfigurationCommandLineDemo": {
            "commandName": "Project",
            "commandLineArgs": "CommandLineKey1=value1 --CommandLineKey2=value2 /CommandLineKey3=value3"
            }
        }
    }
  ```

运行项目，可以看到控制台打印出对应的键值对，类似以下信息：
```
CommandLineKey1:value1
CommandLineKey2:value2
CommandLineKey3:value3
```


## 测试命令替换
为`Program.Main`方法添加命令替换映射，代码如下：
``` csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ConfigurationCommandLineDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            var mapper = new Dictionary<string, string>() {{"-k1", "CommandLineKey1"}};
            builder.AddCommandLine(args, mapper);
            var configurationRoot = builder.Build();
            Console.WriteLine($"CommandLineKey1:{configurationRoot["CommandLineKey1"]}");
            Console.WriteLine($"CommandLineKey2:{configurationRoot["CommandLineKey2"]}");
            Console.WriteLine($"CommandLineKey3:{configurationRoot["CommandLineKey3"]}");
            Console.ReadKey();
        }
    }
}
```
mapper表示用`-k1`这个命令可以代替`CommandLineKey1`
将应用参数修改为：
```
CommandLineKey1=value1 --CommandLineKey2=value2 /CommandLineKey3=value3 -k1=value4
```
运行项目会在控制台打印出以下信息：
```
CommandLineKey1:value4
CommandLineKey2:value2
CommandLineKey3:value3
```
可以发现`CommandLineKey1`原本的值`value1`被后面的`-k1`的值`value4`所替换了，说明了替换规则生效