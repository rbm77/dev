using Buslogix.Interfaces;
using Buslogix.Models;

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
