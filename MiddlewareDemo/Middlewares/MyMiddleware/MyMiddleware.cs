using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MiddlewareDemo.Middlewares.MyMiddleware
{
    class MyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MyMiddleware> _logger;

        public MyMiddleware(RequestDelegate next,ILogger<MyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (_logger.BeginScope("TraceIdentifier:{TraceIdentifier}",context.TraceIdentifier))
            {
                _logger.LogDebug("开始执行MyMiddleware中间件");
                await _next(context);
                _logger.LogDebug("执行MyMiddleware中间件结束");
            }
        }
    }
}
