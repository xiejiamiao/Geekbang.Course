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
