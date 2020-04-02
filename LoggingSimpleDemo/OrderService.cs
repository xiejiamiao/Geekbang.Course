using System;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;

        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public void Show()
        {
            _logger.LogInformation("Show Time {time}",DateTime.Now);
        }
    }
}
