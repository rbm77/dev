using static Pos.Utilities.Enums;

namespace Pos.Interfaces
{
    public interface ILogManager
    {
        Task Log(string message, LogType type);
    }
}
