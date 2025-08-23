using Buslogix.Interfaces;
using Buslogix.Utilities;

namespace Buslogix.Handlers
{
    public class LogHandler : ILogHandler
    {
        public Task WriteLog(string log, Enums.LogType logType)
        {
            return Task.FromResult(0);
        }
    }
}
