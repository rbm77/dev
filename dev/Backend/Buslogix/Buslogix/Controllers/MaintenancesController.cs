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

        private readonly IMaintenanceService _maintenanceService = maintenanceService;

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingMaintenances(
            [FromQuery] int? vehicleId = null,
            [FromQuery] MaintenanceType? type = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Maintenance> maintenances = await _maintenanceService.GetPendingMaintenances(companyId, vehicleId, type);
            return Ok(maintenances);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedMaintenances(
            [FromQuery] int? vehicleId = null,
            [FromQuery] MaintenanceType? type = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Maintenance> maintenances = await _maintenanceService.GetCompletedMaintenances(companyId, vehicleId, type);
            return Ok(maintenances);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMaintenance(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Maintenance? maintenance = await _maintenanceService.GetMaintenance(companyId, id);
            return maintenance == null ? NotFound() : Ok(maintenance);
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertMaintenance([FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _maintenanceService.InsertMaintenance(companyId, maintenance);
            return id > 0 ? CreatedAtAction(nameof(GetMaintenance), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMaintenance(int id, [FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _maintenanceService.UpdateMaintenance(companyId, id, maintenance);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _maintenanceService.DeleteMaintenance(companyId, id);
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.MAINTENANCE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteMaintenance(int id, [FromBody] Maintenance maintenance)
        {
            int companyId = HttpContext.GetCompanyId();
            bool completed = await _maintenanceService.CompleteMaintenance(companyId, id, maintenance);
            return completed ? NoContent() : NotFound();
        }
    }
}