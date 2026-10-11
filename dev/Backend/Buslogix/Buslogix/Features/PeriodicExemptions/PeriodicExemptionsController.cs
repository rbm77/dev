using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.PeriodicExemptions
{
    [Route("periodic-exemptions")]
    [ApiController]
    public class PeriodicExemptionsController(IPeriodicExemptionRepository periodicExemptionRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPeriodicExemptions(
            [FromQuery] int? studentId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<PeriodicExemption> periodicExemptions =
                await periodicExemptionRepository.GetPeriodicExemptions(companyId, studentId, page, pageSize);
            return Ok(periodicExemptions);
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPeriodicExemption(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            PeriodicExemption? periodicExemption = await periodicExemptionRepository.GetPeriodicExemption(companyId, id);
            return periodicExemption == null ? NotFound() : Ok(periodicExemption);
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPeriodicExemption([FromBody] PeriodicExemption periodicExemption)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await periodicExemptionRepository.InsertPeriodicExemption(companyId, periodicExemption);
            return id > 0 ? CreatedAtAction(nameof(GetPeriodicExemption), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePeriodicExemption(int id, [FromBody] PeriodicExemption periodicExemption, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: updating PeriodicExemption {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool updated = (await periodicExemptionRepository.UpdatePeriodicExemption(companyId, id, periodicExemption)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.PERIODIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePeriodicExemption(int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting PeriodicExemption {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await periodicExemptionRepository.DeletePeriodicExemption(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}