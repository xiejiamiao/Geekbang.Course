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
            var configurationRoot = builder.Build();

            Console.WriteLine($"Key1={configurationRoot["Key1"]}");
            Console.WriteLine($"Key2={configurationRoot["Key2"]}");
            Console.WriteLine($"Key3={configurationRoot["Key3"]}");
            Console.WriteLine($"Key4={configurationRoot["Key4"]}");
        }
    }
}
