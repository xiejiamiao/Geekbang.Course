using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionScopeAndDisposableDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionScopeAndDisposableDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        //public int Get([FromServices] IOrderService orderService1, [FromServices] IOrderService orderService2,[FromServices] IHostApplicationLifetime hostApplicationLifetime,[FromQuery]bool isStop=false)
        public int Get([FromServices] IHostApplicationLifetime hostApplicationLifetime, [FromQuery]bool isStop = false)
        {
            //Console.WriteLine("========1==========");
            //using (var scope = HttpContext.RequestServices.CreateScope()) //HttpContext.RequestServices=当前应用程序请求的根容器  .CreateScope=再创建一个子容器来获取服务
            //{
            //    var service = scope.ServiceProvider.GetService<IOrderService>(); // 从子容器获取到IOrderService类型的服务
            //    // 使用using为了测试作用域释放了，作用域里的对象是否跟着一起释放
            //}
            //Console.WriteLine("========2==========");


            if (isStop)
            {
                hostApplicationLifetime.StopApplication();
            }

            Console.WriteLine("=====接口请求处理结束====");
            return 1;
        }
    }
}
