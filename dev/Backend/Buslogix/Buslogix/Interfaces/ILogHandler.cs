using static Buslogix.Models.Enums;

namespace Buslogix.Interfaces
{
    public interface ILogHandler
    {
        Task WriteLog(string log, LogType logType);
    }
}
