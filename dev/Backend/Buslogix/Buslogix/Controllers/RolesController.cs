using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetRoles([FromQuery] string? description = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Role> roles = await _roleService.GetRoles(companyId, description);
            return Ok(roles);
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRole(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Role? role = await _roleService.GetRole(companyId, id);
            return role == null ? NotFound() : Ok(role);
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertRole([FromBody] Role role)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _roleService.InsertRole(companyId, role);
            return id > 0 ? CreatedAtAction(nameof(GetRole), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] Role role)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _roleService.UpdateRole(companyId, id, role);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.ROLE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _roleService.DeleteRole(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
