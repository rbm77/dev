using System.Security.Claims;
using Buslogix.Models;
using Microsoft.AspNetCore.Authorization;

namespace Buslogix.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            Claim? claim = context.User.FindFirst("permissions");
            if (claim != null)
            {
                string[] permissions = claim.Value.Split(',');

                string view = requirement.RequiredPermission.Split('.')[0];
                string editPermission = $"{view}.Edit";

                if (PermissionMap.PermissionToCode.TryGetValue(editPermission, out string? editCode))
                {
                    if (permissions.Contains(editCode))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }

                if (PermissionMap.PermissionToCode.TryGetValue(requirement.RequiredPermission, out string? requiredCode))
                {
                    if (permissions.Contains(requiredCode))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
