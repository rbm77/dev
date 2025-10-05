using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("periodic-exemptions")]
    [ApiController]
    public class PeriodicExemptionsController(IPeriodicExemptionService periodicExemptionService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPeriodicExemptions([FromQuery] int? studentId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<PeriodicExemption> periodicExemptions =
                await periodicExemptionService.GetPeriodicExemptions(companyId, studentId);
            return Ok(periodicExemptions);
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPeriodicExemption(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            PeriodicExemption? periodicExemption = await periodicExemptionService.GetPeriodicExemption(companyId, id);
            return periodicExemption == null ? NotFound() : Ok(periodicExemption);
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPeriodicExemption([FromBody] PeriodicExemption periodicExemption)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await periodicExemptionService.InsertPeriodicExemption(companyId, periodicExemption);
            return id > 0 ? CreatedAtAction(nameof(GetPeriodicExemption), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePeriodicExemption(int id, [FromBody] PeriodicExemption periodicExemption, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await periodicExemptionService.UpdatePeriodicExemption(companyId, id, periodicExemption);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePeriodicExemption(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await periodicExemptionService.DeletePeriodicExemption(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}