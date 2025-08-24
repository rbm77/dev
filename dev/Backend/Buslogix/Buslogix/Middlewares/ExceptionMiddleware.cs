using System.Text.Json;
using Buslogix.Interfaces;
using Buslogix.Models.DTO;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogHandler logHandler)
    {

        private readonly RequestDelegate _next = next;
        private readonly ILogHandler _logHandler = logHandler;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string log = ex.Message;
                if (ex.InnerException != null)
                {
                    log = string.Concat(log, " | ", ex.InnerException.Message);
                }
                await _logHandler.WriteLog(log, LogType.Error);
                await WriteResponse(context);
            }
        }

        private static Task WriteResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            Error response = new() { Message = "An unexpected error occurred." };
            string jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
