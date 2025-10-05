using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("specific-exemptions")]
    [ApiController]
    public class SpecificExemptionsController(ISpecificExemptionService specificExemptionService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetSpecificExemptions(
            [FromQuery] int? studentId = null,
            [FromQuery] int? paymentPeriodId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<SpecificExemption> specificExemptions =
                await specificExemptionService.GetSpecificExemptions(companyId, studentId, paymentPeriodId);
            return Ok(specificExemptions);
        }

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSpecificExemption(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            SpecificExemption? specificExemption = await specificExemptionService.GetSpecificExemption(companyId, id);
            return specificExemption == null ? NotFound() : Ok(specificExemption);
        }

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertSpecificExemption([FromBody] SpecificExemption specificExemption)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await specificExemptionService.InsertSpecificExemption(companyId, specificExemption);
            return id > 0 ? CreatedAtAction(nameof(GetSpecificExemption), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSpecificExemption(int id, [FromBody] SpecificExemption specificExemption, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await specificExemptionService.UpdateSpecificExemption(companyId, id, specificExemption);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSpecificExemption(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await specificExemptionService.DeleteSpecificExemption(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}