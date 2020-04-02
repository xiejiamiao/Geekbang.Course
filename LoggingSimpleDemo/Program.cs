using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configurationRoot = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfigurationRoot>(p => configurationRoot);
            serviceCollection.AddTransient<OrderService>();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConfiguration(configurationRoot.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            /*
            var loggerA = loggerFactory.CreateLogger("LoggerA");
            
            loggerA.LogDebug(2001,"This is LoggerA -- logDebug");
            loggerA.LogInformation("This is LoggerA -- logInformation");
            loggerA.LogError(new Exception("LoggerA Error"),"This is LoggerA -- LogError");

            var orderService = serviceProvider.GetService<OrderService>();
            orderService.Show();
            */
            var logger = loggerFactory.CreateLogger<Program>();
            using (var scope = logger.BeginScope("scopeId={scopeId}",Guid.NewGuid()))
            {
                logger.LogTrace("This is Trace in scope");
                logger.LogInformation("This is Information in scope");
                logger.LogWarning("This is Warning in scope");
                logger.LogError("This is Error in scope");
            }

            Console.ReadKey();
        }
    }
}
