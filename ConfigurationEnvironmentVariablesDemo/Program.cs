using System;
using Microsoft.Extensions.Configuration;

namespace ConfigurationEnvironmentVariablesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            //builder.AddEnvironmentVariables();

            //var configurationRoot = builder.Build();
            //Console.WriteLine($"KEY3={configurationRoot["KEY3"]}");


            //var section = configurationRoot.GetSection("SECTIONA");
            //Console.WriteLine($"SECTIONA:KEY2={section["KEY2"]}");

            #region 前缀过滤

            builder.AddEnvironmentVariables("DIMSUM_");
            var configurationRoot = builder.Build();
            Console.WriteLine($"DIMSUM_KEY1={configurationRoot["KEY1"]}");

            #endregion
        }
    }
}
