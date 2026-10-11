using Buslogix.Common.Logging;

namespace Buslogix.Common.Middlewares
{
    public class RequestIdMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            context.Items["RequestId"] = LogScope.NewId();
            await next(context);
        }
    }
}
