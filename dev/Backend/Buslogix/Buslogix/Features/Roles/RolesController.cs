using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Roles
{
    [Route("[controller]")]
    [ApiController]
    public class RolesController(IRoleRepository roleRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetRoles(
            [FromQuery] string? description = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Role> roles = await roleRepository.GetRoles(companyId, description, page, pageSize);
            return Ok(roles);
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRole(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Role? role = await roleRepository.GetRole(companyId, id);
            return role == null ? NotFound() : Ok(role);
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertRole([FromBody] Role role)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await roleRepository.InsertRole(companyId, role);
            return id > 0 ? CreatedAtAction(nameof(GetRole), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] Role role)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await roleRepository.UpdateRole(companyId, id, role)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole(int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting Role {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await roleRepository.DeleteRole(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.READ}")]
        [HttpGet("{roleId:int}/permissions")]
        public async Task<IActionResult> GetPermissions(
            int roleId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<RolePermission> permissions = await roleRepository.GetPermissions(companyId, roleId, page, pageSize);
            return Ok(permissions);
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.WRITE}")]
        [HttpPut("{roleId:int}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int roleId, [FromBody] List<RolePermission> permissions, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: updating permissions for Role {roleId} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            string permissionsJson = JsonSerializer.Serialize(permissions);
            bool updated = (await roleRepository.UpdatePermissions(companyId, roleId, permissionsJson)) > 0;
            return updated ? NoContent() : NotFound();
        }
    }
}
