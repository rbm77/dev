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

        private readonly IIncidentService _incidentService = incidentService;

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetIncidents(
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null,
            [FromQuery] IncidentType? type = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Incident> incidents = await _incidentService.GetIncidents(companyId, vehicleId, driverId, type);
            return Ok(incidents);
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetIncident(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Incident? incident = await _incidentService.GetIncident(companyId, id);
            return incident == null ? NotFound() : Ok(incident);
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertIncident([FromBody] Incident incident)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _incidentService.InsertIncident(companyId, incident);
            return id > 0 ? CreatedAtAction(nameof(GetIncident), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateIncident(int id, [FromBody] Incident incident)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _incidentService.UpdateIncident(companyId, id, incident);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _incidentService.DeleteIncident(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}