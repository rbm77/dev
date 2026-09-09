
namespace Buslogix.Common.Logging
{
    public class LogHandler : ILogHandler
    {
        public Task WriteLog(string log, Enums.LogType logType)
        {
            return Task.FromResult(0);
        }
    }
}
