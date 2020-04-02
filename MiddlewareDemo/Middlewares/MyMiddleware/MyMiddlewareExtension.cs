using Microsoft.AspNetCore.Builder;

namespace MiddlewareDemo.Middlewares.MyMiddleware
{
    public static class MyMiddlewareExtension
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MyMiddleware>();
        }
    }
}
