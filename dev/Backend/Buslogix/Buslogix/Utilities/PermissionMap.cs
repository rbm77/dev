namespace Buslogix.Utilities
{
    public static class PermissionMap
    {
        private static readonly Dictionary<string, string> CodeToPermission = new()
        {
            { "1", $"{Resources.COMPANY}.{PermissionMode.WRITE}" },
            { "2", $"{Resources.COMPANY}.{PermissionMode.READ}" },
            { "3", $"{Resources.ROLE}.{PermissionMode.WRITE}" },
            { "4", $"{Resources.ROLE}.{PermissionMode.READ}" },
            { "5", $"{Resources.USER}.{PermissionMode.WRITE}" },
            { "6", $"{Resources.USER}.{PermissionMode.READ}" },
            { "7", $"{Resources.OWN_USER}.{PermissionMode.WRITE}" },
            { "8", $"{Resources.OWN_USER}.{PermissionMode.READ}" },
            { "9", $"{Resources.EMPLOYEE}.{PermissionMode.WRITE}" },
            { "10", $"{Resources.EMPLOYEE}.{PermissionMode.READ}" }
        };

        public static readonly Dictionary<string, string> PermissionToCode =
            CodeToPermission.ToDictionary(static kvp => kvp.Value, static kvp => kvp.Key);
    }
}
