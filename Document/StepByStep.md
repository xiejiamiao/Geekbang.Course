---
title: 特基础教程系列
date: 2020/04/01 22:18:00
tag:
- Visual Studio Code
- 特基础
categories:
- StepByStep
---


- [Visual Studio Code](#visual-studio-code)
  - [下载安装](#%e4%b8%8b%e8%bd%bd%e5%ae%89%e8%a3%85)
  - [关于安装扩展插件](#%e5%85%b3%e4%ba%8e%e5%ae%89%e8%a3%85%e6%89%a9%e5%b1%95%e6%8f%92%e4%bb%b6)
  - [关于汉化](#%e5%85%b3%e4%ba%8e%e6%b1%89%e5%8c%96)
- [平时写静态页流程](#%e5%b9%b3%e6%97%b6%e5%86%99%e9%9d%99%e6%80%81%e9%a1%b5%e6%b5%81%e7%a8%8b)
- [todo:常用的`vscode`插件](#todo%e5%b8%b8%e7%94%a8%e7%9a%84vscode%e6%8f%92%e4%bb%b6)
- [todo:常用的`vscode`代码段](#todo%e5%b8%b8%e7%94%a8%e7%9a%84vscode%e4%bb%a3%e7%a0%81%e6%ae%b5)

# Visual Studio Code
## 下载安装
下载地址：[https://code.visualstudio.com/](https://code.visualstudio.com/)，下载之后双击安装即可
## 关于安装扩展插件
如下图，点击左边的插件tab，输入插件名称进行搜索，然后点击`install`即可进行安装
![微信截图_20200401235919.png](https://i.loli.net/2020/04/01/ONqmZth8iAbfECX.png)
## 关于汉化
`vscode`的汉化包使用的也是通过插件的形式来安装，搜索`chinese`，找到`Chinese (Simplified) Language Pack for Visual Studio Code`这个插件进行安装，安装完重启即可

# 平时写静态页流程
1. 一般我会在电脑指定一个盘创建一个文件夹，指定这个文件夹存放自己的源码，无论个人电脑还是公司电脑，比如`G`盘里创建了`repo`的文件夹作为我统一管理源码的文件夹
2. 通常自己写代码会分两种级别，一种是为了学习测试而写的demo，一种是比较有一定业务逻辑需求的小项目，所以一般会在`repo`文件夹下再创建两个文件夹，分别为`jiamiao.x.demo`和`jiamiao.x.project`
3. 注意一点，**无论是在写`demo`还是写`project`，都不要出现类似`demo1`、`demo2`、`project1`、`project2`这种没意义的名字**，以下以我写一段表单`demo`为例子：
4. 在`G:/repo/jiamiao.x.demo`的文件夹中创建文件夹`form_demo`，然后再`form_demo`文件夹上右键，点击`通过Code打开`
5. 打开之后可以看到左边有文件夹的目录，当前现在是一片空白，如下图：
   
   ![微信截图_20200402002810.png](https://i.loli.net/2020/04/02/TjLXCdGEiIoOxSR.png)
6. 接下来在`vscode`中操作，在`FORM_DEMO`下方空白处右键，点击`新建文件`，输入`index.html`回车，可以看到创建了`index.html`文件，在右边的编辑区则可以进行编码，只要文件扩展名`.html`正确，则可以代码高亮和智能提示
7. 当静态页需要引用图片、样式、脚本等，可以在`FORM_DEMO`下方空白处右键，点击`新建文件夹`，对应输入`image/style/script`等名字之后回车即可
8. 写完页面之后，可以右键`index.html`文件，点击`Open In Default Browser`(需要安装插件`Open in Browser`)，直接打开浏览器来查看页面效果，如下图：
   
   ![dcfc42a87e4a4804531ffe3de29fd60.png](https://i.loli.net/2020/04/02/kaPc2qStsbyi3DI.png)

# todo:常用的`vscode`插件
# todo:常用的`vscode`代码段