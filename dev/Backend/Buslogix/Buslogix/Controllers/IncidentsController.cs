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
    public class IncidentsController(IIncidentService incidentService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetIncidents(
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null,
            [FromQuery] IncidentType? type = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Incident> incidents = await incidentService.GetIncidents(companyId, vehicleId, driverId, type);
            return Ok(incidents);
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetIncident(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Incident? incident = await incidentService.GetIncident(companyId, id);
            return incident == null ? NotFound() : Ok(incident);
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertIncident([FromBody] Incident incident)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await incidentService.InsertIncident(companyId, incident);
            return id > 0 ? CreatedAtAction(nameof(GetIncident), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateIncident(int id, [FromBody] Incident incident)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await incidentService.UpdateIncident(companyId, id, incident);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteIncident(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await incidentService.DeleteIncident(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}