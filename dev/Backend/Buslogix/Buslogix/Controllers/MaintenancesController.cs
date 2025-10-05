using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MaintenancesController(IMaintenanceService maintenanceService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingMaintenances(
            [FromQuery] int? vehicleId = null,
            [FromQuery] MaintenanceType? type = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Maintenance> maintenances = await maintenanceService.GetPendingMaintenances(companyId, vehicleId, type);
            return Ok(maintenances);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedMaintenances(
            [FromQuery] int? vehicleId = null,
            [FromQuery] MaintenanceType? type = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Maintenance> maintenances = await maintenanceService.GetCompletedMaintenances(companyId, vehicleId, type);
            return Ok(maintenances);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMaintenance(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Maintenance? maintenance = await maintenanceService.GetMaintenance(companyId, id);
            return maintenance == null ? NotFound() : Ok(maintenance);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertMaintenance([FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await maintenanceService.InsertMaintenance(companyId, maintenance);
            return id > 0 ? CreatedAtAction(nameof(GetMaintenance), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMaintenance(int id, [FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await maintenanceService.UpdateMaintenance(companyId, id, maintenance);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMaintenance(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await maintenanceService.DeleteMaintenance(companyId, id);
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteMaintenance(int id, [FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            bool completed = await maintenanceService.CompleteMaintenance(companyId, id, maintenance);
            return completed ? NoContent() : NotFound();
        }
    }
}