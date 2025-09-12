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
            { "10", $"{Resources.EMPLOYEE}.{PermissionMode.READ}" },
            { "11", $"{Resources.SALARY}.{PermissionMode.WRITE}" },
            { "12", $"{Resources.SALARY}.{PermissionMode.READ}" },
            { "13", $"{Resources.VEHICLE}.{PermissionMode.WRITE}" },
            { "14", $"{Resources.VEHICLE}.{PermissionMode.READ}" },
            { "15", $"{Resources.ROUTE}.{PermissionMode.WRITE}" },
            { "16", $"{Resources.ROUTE}.{PermissionMode.READ}" },
            { "17", $"{Resources.GRADE}.{PermissionMode.WRITE}" },
            { "18", $"{Resources.GRADE}.{PermissionMode.READ}" },
            { "19", $"{Resources.STUDENT}.{PermissionMode.WRITE}" },
            { "20", $"{Resources.STUDENT}.{PermissionMode.READ}" },
            { "21", $"{Resources.CONTACT}.{PermissionMode.WRITE}" },
            { "22", $"{Resources.CONTACT}.{PermissionMode.READ}" }
        };

        public static readonly Dictionary<string, string> PermissionToCode =
            CodeToPermission.ToDictionary(static kvp => kvp.Value, static kvp => kvp.Key);
    }
}
