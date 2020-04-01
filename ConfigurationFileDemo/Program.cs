using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationFileDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appSetting.json", optional: false, reloadOnChange: true); //optional:文件是否可选  reloadOnChange:更新文件自动重新获取

            /*
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
            */

            /*
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
            */

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
                    options.BindNonPublicProperties = true; //会将配置项的值绑定到私有属性上，默认为false
                });
            Console.WriteLine($"Key1={config.Key1}");
            Console.WriteLine($"Key3={config.Key3}");
            Console.WriteLine($"Key4={config.Key4}");
            Console.WriteLine($"Key5={config.Key5}");
        }
    }

    class Config
    {
        public string Key1 { get; set; }

        public bool Key3 { get; set; }

        public int Key4 { get; set; }

        public string Key5 { get; private set; }

    }
}
