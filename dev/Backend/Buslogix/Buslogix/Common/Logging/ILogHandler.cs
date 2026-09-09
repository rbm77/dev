using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Common.Logging
{
    public interface ILogHandler
    {
        Task WriteLog(string log, LogType logType);
    }
}
