using System.Security.Cryptography;

namespace Buslogix.Common.Logging
{
    public static class LogScope
    {
        private static readonly AsyncLocal<string?> _currentId = new();
        private static readonly AsyncLocal<int?> _currentCompanyId = new();

        public static string? CurrentId => _currentId.Value;

        public static int? CurrentCompanyId => _currentCompanyId.Value;

        public static string NewId() =>
            Convert.ToHexString(RandomNumberGenerator.GetBytes(6)).ToLowerInvariant();

        public static IDisposable Push(string id, int? companyId)
        {
            (string?, int?) previous = (_currentId.Value, _currentCompanyId.Value);
            _currentId.Value = id;
            _currentCompanyId.Value = companyId;
            return new Popper(previous);
        }

        private sealed class Popper((string?, int?) previous) : IDisposable
        {
            public void Dispose()
            {
                _currentId.Value = previous.Item1;
                _currentCompanyId.Value = previous.Item2;
            }
        }
    }
}
