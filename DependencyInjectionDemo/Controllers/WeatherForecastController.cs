using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionDemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IOrderService orderService,IGenericService<IOrderService> genericService)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public int GetService([FromServices] IMySingletonService singletonService1,
            [FromServices] IMySingletonService singletonService2,
            [FromServices] IMyScopeService scopeService1,
            [FromServices] IMyScopeService scopeService2,
            [FromServices] IMyTransientService transientService1,
            [FromServices] IMyTransientService transientService2)
        {
            Console.WriteLine($"{nameof(singletonService1)}:{singletonService1.GetHashCode()}");
            Console.WriteLine($"{nameof(singletonService2)}:{singletonService2.GetHashCode()}");

            Console.WriteLine($"{nameof(scopeService1)}:{scopeService1.GetHashCode()}");
            Console.WriteLine($"{nameof(scopeService2)}:{scopeService2.GetHashCode()}");

            Console.WriteLine($"{nameof(transientService1)}:{transientService1.GetHashCode()}");
            Console.WriteLine($"{nameof(transientService2)}:{transientService2.GetHashCode()}");

            Console.WriteLine("=========请求结束========");
            return 1;
        }

        [HttpGet]
        public int GetServiceList([FromServices] IEnumerable<IOrderService> orderServices)
        {
            foreach (var item in orderServices)
            {
                Console.WriteLine($"获取到服务实例：{item.ToString()}:{item.GetHashCode()}");
            }

            return 1;
        }
    }
}
