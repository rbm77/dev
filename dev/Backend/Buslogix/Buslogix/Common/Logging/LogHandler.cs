using Microsoft.Extensions.Logging;
using Serilog.Context;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Common.Logging
{
    public class LogHandler(ILogger<LogHandler> logger, IHttpContextAccessor httpContextAccessor) : ILogHandler
    {
        public Task WriteLog(string log, LogType logType)
        {
            HttpContext? context = httpContextAccessor.HttpContext;

            string requestId = context?.Items["RequestId"] as string
                ?? LogScope.CurrentId
                ?? "-";

            int? companyId = context?.Items["CompanyId"] as int?
                ?? LogScope.CurrentCompanyId;

            string companyText = companyId.HasValue ? companyId.Value.ToString() : "-";

            using (LogContext.PushProperty("RequestId", requestId))
            using (LogContext.PushProperty("CompanyId", companyText))
            {
                switch (logType)
                {
                    case LogType.Warning:
                        logger.LogWarning("{LogMessage}", log);
                        break;
                    case LogType.Error:
                        logger.LogError("{LogMessage}", log);
                        break;
                    default:
                        logger.LogInformation("{LogMessage}", log);
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
