using Microsoft.AspNetCore.Authorization;

namespace Buslogix.Common.Auth
{
    public class PermissionRequirement(string requiredPermission) : IAuthorizationRequirement
    {
        public string RequiredPermission { get; } = requiredPermission;
    }
}
