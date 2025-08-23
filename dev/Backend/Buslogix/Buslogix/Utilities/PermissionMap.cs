namespace Buslogix.Utilities
{
    public static class PermissionMap
    {
        private static readonly Dictionary<string, string> CodeToPermission = new()
        {
            { "1", $"{Resources.COMPANY}.{PermissionMode.WRITE}" },
            { "2", $"{Resources.COMPANY}.{PermissionMode.READ}" },
        };

        public static readonly Dictionary<string, string> PermissionToCode =
            CodeToPermission.ToDictionary(static kvp => kvp.Value, static kvp => kvp.Key);
    }
}
