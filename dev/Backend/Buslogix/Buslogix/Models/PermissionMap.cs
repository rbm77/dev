namespace Buslogix.Models
{
    public static class PermissionMap
    {
        private static readonly Dictionary<string, string> CodeToPermission = new()
        {
            { "1", "Company.Edit" },
            { "2", "Company.Read" },
        };

        public static readonly Dictionary<string, string> PermissionToCode =
            CodeToPermission.ToDictionary(static kvp => kvp.Value, static kvp => kvp.Key);
    }
}
