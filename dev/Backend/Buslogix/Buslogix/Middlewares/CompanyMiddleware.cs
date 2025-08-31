using System.Text.Json;
using Buslogix.Models.DTO;

namespace Buslogix.Middlewares
{
    public class CompanyMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {

            if (context.User?.Identity?.IsAuthenticated == false)
            {
                await _next(context);
                return;
            }

            string? companyId = context.User?.FindFirst("cid")?.Value;

            if (string.IsNullOrEmpty(companyId))
            {
                await WriteResponse(context);
                return;
            }

            context.Items["CompanyId"] = int.Parse(companyId);

            await _next(context);
        }

        private static Task WriteResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            Error response = new() { Message = "Company claim missing." };
            string jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
