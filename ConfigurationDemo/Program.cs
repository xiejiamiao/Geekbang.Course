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

            Console.WriteLine("Hello World!");
        }
    }
}
