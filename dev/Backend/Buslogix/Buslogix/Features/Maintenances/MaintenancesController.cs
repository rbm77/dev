using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Maintenances
{
    [Route("[controller]")]
    [ApiController]
    public class MaintenancesController(IMaintenanceRepository maintenanceRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingMaintenances(
            [FromQuery] int? vehicleId = null,
            [FromQuery] MaintenanceType? type = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Maintenance> maintenances = await maintenanceRepository.GetPendingMaintenances(companyId, vehicleId, type, page, pageSize);
            return Ok(maintenances);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedMaintenances(
            [FromQuery] int? vehicleId = null,
            [FromQuery] MaintenanceType? type = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Maintenance> maintenances = await maintenanceRepository.GetCompletedMaintenances(companyId, vehicleId, type, page, pageSize);
            return Ok(maintenances);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMaintenance(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Maintenance? maintenance = await maintenanceRepository.GetMaintenance(companyId, id);
            return maintenance == null ? NotFound() : Ok(maintenance);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertMaintenance([FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await maintenanceRepository.InsertMaintenance(companyId, maintenance);
            return id > 0 ? CreatedAtAction(nameof(GetMaintenance), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMaintenance(int id, [FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await maintenanceRepository.UpdateMaintenance(companyId, id, maintenance)) > 0;
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
            bool deleted = (await maintenanceRepository.DeleteMaintenance(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteMaintenance(int id, [FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            bool completed = (await maintenanceRepository.CompleteMaintenance(companyId, id, maintenance)) > 0;
            return completed ? NoContent() : NotFound();
        }
    }
}