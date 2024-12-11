using Pos.Interfaces;
using static Pos.Utilities.Enums;

namespace Pos.Utilities
{
    public class LogManager : ILogManager
    {
        public async Task Log(string message, LogType type)
        {
            await Task.Delay(1000);
        }
    }
}
