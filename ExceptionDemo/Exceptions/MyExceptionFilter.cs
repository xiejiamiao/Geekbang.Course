using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExceptionDemo.Exceptions
{
    public class MyExceptionFilter:IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var knownException = context.Exception as IKnownException;
            if (knownException == null)
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<MyExceptionFilter>>();
                logger.LogError(context.Exception,context.Exception.Message);
                knownException = KnownException.UnKnown;
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            else
            {
                knownException = KnownException.FromKnownException(knownException);
                context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            }

            context.Result = new JsonResult(knownException)
            {
                ContentType = "application/json; charset=utf-8"
            };
        }
    }
}
