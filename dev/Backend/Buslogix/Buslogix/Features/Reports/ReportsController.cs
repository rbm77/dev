using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.Reports
{
    [Route("reports")]
    [ApiController]
    public class ReportsController(IReportRepository reportRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.DEBTOR}.{PermissionMode.READ}")]
        [HttpGet("debtors")]
        public async Task<IActionResult> GetDebtors(
            [FromQuery] int? routeId = null,
            [FromQuery] int? studentId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Debtor> result = await reportRepository.GetDebtors(companyId, routeId, studentId, isActive, page, pageSize);
            return Ok(result);
        }
    }
}
