using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Incidents
{
    [Route("[controller]")]
    [ApiController]
    public class IncidentsController(IIncidentRepository incidentRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetIncidents(
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null,
            [FromQuery] IncidentType? type = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Incident> incidents = await incidentRepository.GetIncidents(companyId, vehicleId, driverId, type, page, pageSize);
            return Ok(incidents);
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetIncident(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Incident? incident = await incidentRepository.GetIncident(companyId, id);
            return incident == null ? NotFound() : Ok(incident);
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertIncident([FromBody] Incident incident)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await incidentRepository.InsertIncident(companyId, incident);
            return id > 0 ? CreatedAtAction(nameof(GetIncident), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateIncident(int id, [FromBody] Incident incident)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await incidentRepository.UpdateIncident(companyId, id, incident)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.INCIDENT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteIncident(int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting Incident {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await incidentRepository.DeleteIncident(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}