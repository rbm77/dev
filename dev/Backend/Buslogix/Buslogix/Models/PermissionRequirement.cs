using Microsoft.AspNetCore.Authorization;

namespace Buslogix.Models
{
    public class PermissionRequirement(string requiredPermission) : IAuthorizationRequirement
    {
        public string RequiredPermission { get; } = requiredPermission;
    }
}
