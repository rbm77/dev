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
            { "22", $"{Resources.CONTACT}.{PermissionMode.READ}" },
            { "23", $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}" },
            { "24", $"{Resources.MAINTENANCE}.{PermissionMode.READ}" },
            { "25", $"{Resources.INCIDENT}.{PermissionMode.WRITE}" },
            { "26", $"{Resources.INCIDENT}.{PermissionMode.READ}" },
            { "27", $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}" },
            { "28", $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.READ}" },
            { "29", $"{Resources.EXPENSE}.{PermissionMode.WRITE}" },
            { "30", $"{Resources.EXPENSE}.{PermissionMode.READ}" },
            { "31", $"{Resources.PAYMENT_PERIOD}.{PermissionMode.WRITE}" },
            { "32", $"{Resources.PAYMENT_PERIOD}.{PermissionMode.READ}" },
            { "33", $"{Resources.VACATION}.{PermissionMode.WRITE}" },
            { "34", $"{Resources.VACATION}.{PermissionMode.READ}" },
            { "35", $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.WRITE}" },
            { "36", $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.READ}" },
            { "37", $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.WRITE}" },
            { "38", $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.READ}" },
            { "39", $"{Resources.PAYMENT}.{PermissionMode.WRITE}" },
            { "40", $"{Resources.PAYMENT}.{PermissionMode.READ}" }

        };

        public static readonly Dictionary<string, string> PermissionToCode =
            CodeToPermission.ToDictionary(static kvp => kvp.Value, static kvp => kvp.Key);
    }
}
