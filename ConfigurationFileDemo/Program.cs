using System;
using Microsoft.Extensions.Configuration;

namespace ConfigurationFileDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appSetting.json",optional:false,reloadOnChange:true); //optional:文件是否可选  reloadOnChange:更新文件自动重新获取
            builder.AddIniFile("appSetting.ini", optional: false, reloadOnChange: true);
            var configurationRoot = builder.Build();
            Console.WriteLine("开始了。。输入随意字符串返回配置项，直接回车推出");
            var isStop = Console.ReadLine();
            while (!string.IsNullOrEmpty(isStop))
            {
                Console.WriteLine($"Key1={configurationRoot["Key1"]}");
                Console.WriteLine($"Key2={configurationRoot["Key2"]}");
                Console.WriteLine($"Key3={configurationRoot["Key3"]}");
                Console.WriteLine($"Key4={configurationRoot["Key4"]}");
                Console.WriteLine($"Key5={configurationRoot["Key5"]}");
                Console.WriteLine("=====分割线=====");
                isStop = Console.ReadLine();
            }
        }
    }
}
