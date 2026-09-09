using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.SpecificExemptions
{
    [Route("specific-exemptions")]
    [ApiController]
    public class SpecificExemptionsController(ISpecificExemptionRepository specificExemptionRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetSpecificExemptions(
            [FromQuery] int? studentId = null,
            [FromQuery] int? paymentPeriodId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<SpecificExemption> specificExemptions =
                await specificExemptionRepository.GetSpecificExemptions(companyId, studentId, paymentPeriodId, page, pageSize);
            return Ok(specificExemptions);
        }

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSpecificExemption(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            SpecificExemption? specificExemption = await specificExemptionRepository.GetSpecificExemption(companyId, id);
            return specificExemption == null ? NotFound() : Ok(specificExemption);
        }

        [Authorize(Policy = $"{Resources.SPECIFIC_EXEMPTION}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertSpecificExemption([FromBody] SpecificExemption specificExemption)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await specificExemptionRepository.InsertSpecificExemption(companyId, specificExemption);
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
            bool updated = (await specificExemptionRepository.UpdateSpecificExemption(companyId, id, specificExemption)) > 0;
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
            bool deleted = (await specificExemptionRepository.DeleteSpecificExemption(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}