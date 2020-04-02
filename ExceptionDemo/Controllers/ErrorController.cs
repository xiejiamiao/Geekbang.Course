using ExceptionDemo.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExceptionDemo.Controllers
{
    
    public class ErrorController : Controller
    {

        [Route("/error")]
        public IActionResult Index()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = exceptionHandlerPathFeature?.Error;

            var knownException = ex as IKnownException;
            if (knownException == null)
            {
                var logger = HttpContext.RequestServices.GetService<ILogger<ErrorController>>();
                logger.LogError(ex,ex.Message);
                 knownException = KnownException.UnKnown;
            }
            else
            {
                knownException = KnownException.FromKnownException(knownException);
            }

            return View(knownException);
        }
    }
}