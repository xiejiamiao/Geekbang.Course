using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationCustomDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddMyConfiguration();
            var configurationRoot = builder.Build();

            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                var lastTime = configurationRoot["lastTime"];
                Console.WriteLine($"lastTime={lastTime}");
                Console.WriteLine("======");
            });

            Console.ReadKey();
        }
    }
}
