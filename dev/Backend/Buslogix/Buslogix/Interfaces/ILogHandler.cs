using static Buslogix.Utilities.Enums;

namespace Buslogix.Interfaces
{
    public interface ILogHandler
    {
        Task WriteLog(string log, LogType logType);
    }
}
