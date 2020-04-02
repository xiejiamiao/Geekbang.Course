using System.Collections.Generic;
using ExceptionDemo.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExceptionDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MyExceptionFilter]
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
        public IEnumerable<WeatherForecast> Get()
        {
            //throw new Exception("出错了");
            throw new InvalidParameterException(65, "参数有误！！！", new List<string>() {"exception info 1","exception info 2" });
        }
    }
}
