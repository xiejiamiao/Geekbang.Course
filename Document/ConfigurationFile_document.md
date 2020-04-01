---
title: .NET Core开发实战课程备忘(8) -- 文件配置提供程序：自由选择配置的格式
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

- [文件配置提供程序](#%e6%96%87%e4%bb%b6%e9%85%8d%e7%bd%ae%e6%8f%90%e4%be%9b%e7%a8%8b%e5%ba%8f)
- [特性](#%e7%89%b9%e6%80%a7)
- [代码示例](#%e4%bb%a3%e7%a0%81%e7%a4%ba%e4%be%8b)
  - [创建项目](#%e5%88%9b%e5%bb%ba%e9%a1%b9%e7%9b%ae)
  - [创建测试配置项`appSetting.json`](#%e5%88%9b%e5%bb%ba%e6%b5%8b%e8%af%95%e9%85%8d%e7%bd%ae%e9%a1%b9appsettingjson)
  - [获取`appSetting.json`里的配置](#%e8%8e%b7%e5%8f%96appsettingjson%e9%87%8c%e7%9a%84%e9%85%8d%e7%bd%ae)
  - [获取`ini`配置文件](#%e8%8e%b7%e5%8f%96ini%e9%85%8d%e7%bd%ae%e6%96%87%e4%bb%b6)
- [配置热更新操作](#%e9%85%8d%e7%bd%ae%e7%83%ad%e6%9b%b4%e6%96%b0%e6%93%8d%e4%bd%9c)
- [将配置项绑定到强类型上](#%e5%b0%86%e9%85%8d%e7%bd%ae%e9%a1%b9%e7%bb%91%e5%ae%9a%e5%88%b0%e5%bc%ba%e7%b1%bb%e5%9e%8b%e4%b8%8a)
  - [引入绑定包](#%e5%bc%95%e5%85%a5%e7%bb%91%e5%ae%9a%e5%8c%85)
  - [创建测试类](#%e5%88%9b%e5%bb%ba%e6%b5%8b%e8%af%95%e7%b1%bb)
  - [将配置项绑定到指定对象上](#%e5%b0%86%e9%85%8d%e7%bd%ae%e9%a1%b9%e7%bb%91%e5%ae%9a%e5%88%b0%e6%8c%87%e5%ae%9a%e5%af%b9%e8%b1%a1%e4%b8%8a)

# 文件配置提供程序
读取不同文件格式或从不同位置读取配置
* Microsoft.Extensions.Configuration.Ini
* Microsoft.Extensions.Configuration.Json
* Microsoft.Extensions.Configuration.UserSecrets
* Microsoft.Extensions.Configuration.Xml

# 特性
* 指定文件可选、必选
* 指定是否监视文件的变更

# 代码示例
## 创建项目
创建名字为`ConfigurationFileDemo`的`控制台应用`，通过`nuget`引入以下四个包：
```
Microsoft.Extensions.Configuration.Ini
Microsoft.Extensions.Configuration.Json
Microsoft.Extensions.Configuration.UserSecrets
Microsoft.Extensions.Configuration.Xml
```
这里不用引用`Microsoft.Extensions.Configuration`和`Microsoft.Extensions.Configuration.Abstractions`两个基础包，是因为其他包已经包含了两个基础包

## 创建测试配置项`appSetting.json`
在项目根目录创建`appSetting.json`文件，内容如下：
``` json
{
  "Key1": "value1",
  "Key2": "value2",
  "Key3": false,
  "Key4": 10
}
```
* 在`Visual Studio 2019`中通过右键`appSetting.json`文件->属性-复制到输出目录选择`如果较新则复制`
* 在`Visual Studio Code`中通过修改`ConfigurationFileDemo.csproj`文件，新增文件输出到配置，具体代码如下：
  ``` xml
    <Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>netcoreapp3.1</TargetFramework>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="Microsoft.Extensions.Configuration.Ini" Version="3.1.3" />
        <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="3.1.3" />
        <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="3.1.3" />
        <PackageReference Include="Microsoft.Extensions.Configuration.Xml" Version="3.1.3" />
    </ItemGroup>
    <ItemGroup>
        <None Update="appSetting.json">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
    </ItemGroup>
    </Project>
  ```
## 获取`appSetting.json`里的配置
修改`Program.Main`方法，内容如下：
``` csharp
using System;
using Microsoft.Extensions.Configuration;
namespace ConfigurationFileDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appSetting.json",optional:false,reloadOnChange:true);
            var configurationRoot = builder.Build();
            Console.WriteLine("开始了。。输入随意字符串返回配置项，直接回车推出");
            var isStop = Console.ReadLine();
            while (!string.IsNullOrEmpty(isStop))
            {
                Console.WriteLine($"Key1={configurationRoot["Key1"]}");
                Console.WriteLine($"Key2={configurationRoot["Key2"]}");
                Console.WriteLine($"Key3={configurationRoot["Key3"]}");
                Console.WriteLine($"Key4={configurationRoot["Key4"]}");
                Console.WriteLine("=====分割线=====");
                isStop = Console.ReadLine();
            }
        }
    }
}
```

optional：表示文件是否可选，false=没文件会报错，true=可以没有文件

reloadOnChange：表示监视配置文件的变动，配置文件变动会进行重新读取

运行代码，然后在控制台随意输入字符串回车，可以看到以下信息：
```
Key1=value1
Key2=value2
Key3=False
Key4=10
=====分割线=====
```
进入项目根目录/bin/Debug/netcoreapp3.1里，找到`appSetting.json`文件，修改里面的配置值，再回到控制台输入随意字符串回车，可以看到打印出来的值已经有所变化


## 获取`ini`配置文件
在项目根目录创建`appSetting.ini`文件，内容如下：
``` ini
Key4=Hello world
Key5=value5
```
类似`appSetting.json`那样配置拷贝到输出目录，修改`Program.Main`方法，在`AddJsonFile`方法下面新增添加`ini`配置源的调用，代码如下：
``` csharp
builder.AddIniFile("appSetting.ini", optional: false, reloadOnChange: true);
```
运行代码，然后在控制台输入随意字符串回车，可以看到以下信息：
```
Key1=value1
Key2=value2
Key3=False
Key4=Hello world
Key5=value5
=====分割线=====
```
可以看到`ini`文件里的配置已经加载进去了，同时**后面加载的配置项如果与前面已经加载的配置项名称一致，会覆盖掉前面加载的配置项**


# 配置热更新操作
这里只用`appSetting.json`文件变更来触发热更新操作作为示例，将`Program.Main`修改为加载`appSetting.json`，并注册配置更新操作代码，具体代码如下：
``` csharp
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationFileDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appSetting.json", optional: false, reloadOnChange: true);
            var configurationRoot = builder.Build();

            Console.WriteLine($"Key1={configurationRoot["Key1"]}");
            Console.WriteLine($"Key2={configurationRoot["Key2"]}");
            Console.WriteLine($"Key3={configurationRoot["Key3"]}");

            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                Console.WriteLine("配置发生了变化");
                Console.WriteLine($"Key1={configurationRoot["Key1"]}");
                Console.WriteLine($"Key2={configurationRoot["Key2"]}");
                Console.WriteLine($"Key3={configurationRoot["Key3"]}");
            });
            Console.ReadKey();
        }
    }
}
```
运行代码，控制台打出现有配置项的键值对，然后到`/bin/Debug/netcoreapp3.1`里修改`appSetting.json`里配置项的值，可以看到控制台打出了新配置项的键值对

# 将配置项绑定到强类型上
* 支持将配置值绑定到已有对象
* 支持将配置值绑定到私有属性上

## 引入绑定包
使用`nuget`引入以下包
```
Microsoft.Extensions.Configuration.Binder
```
## 创建测试类
创建测试类`Config`，具体代码如下:
``` csharp
class Config
{
    public string Key1 { get; set; }
    public bool Key3 { get; set; }
    public int Key4 { get; set; }
    public string Key5 { get; private set; }
}
```
## 将配置项绑定到指定对象上
修改`Program.Main`方法，具体代码如下：
``` csharp
class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appSetting.json", optional: false, reloadOnChange: true); 
        var configurationRoot = builder.Build();
        var config = new Config()
        {
            Key1 = "default1",
            Key3 = false,
            Key4 = 0
        };
        configurationRoot.Bind(config);
        Console.WriteLine($"Key1={config.Key1}");
        Console.WriteLine($"Key3={config.Key3}");
        Console.WriteLine($"Key4={config.Key4}");
        Console.WriteLine($"Key5={config.Key5}");

        Console.WriteLine("======");
        configurationRoot.GetSection("OrderService").Bind(config, options =>
            {
                options.BindNonPublicProperties = true;
            });
        Console.WriteLine($"Key1={config.Key1}");
        Console.WriteLine($"Key3={config.Key3}");
        Console.WriteLine($"Key4={config.Key4}");
        Console.WriteLine($"Key5={config.Key5}");
    }
}
```
`appSetting.json`修改如下：
``` json
{
  "Key1": "value1",
  "Key2": "value2",
  "Key3": true,
  "Key4": 10,
  "key5": "level1" ,
  "OrderService": {

    "Key1": "order_value1",
    "Key2": "order_value2",
    "Key3": false,
    "Key4": 200,
    "key5": "order_value5" 
  } 
}
```
运行代码，控制台会打印出以下信息：
```
Key1=value1
Key3=True
Key4=10
Key5=
======
Key1=order_value1
Key3=False
Key4=200
Key5=order_value5
```
通过`IConfigurationRoot`的`Bind`方法将配置项绑定到指定对象上，如果是多个层级，则通过`IConfigurationRoot`的`GetSection`筛选出指定的层，再绑定到指定对象上，绑定时可以通过设定`BindNonPublicProperties`这个属性来将配置型绑定到私有属性上，默认配置项是不绑定到私有属性的