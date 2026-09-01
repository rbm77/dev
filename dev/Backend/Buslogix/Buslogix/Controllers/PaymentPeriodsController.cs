using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("payment-periods")]
    [ApiController]
    public class PaymentPeriodsController(IPaymentPeriodService paymentPeriodService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentPeriods(
            [FromQuery] int? requestId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<PaymentPeriod> paymentPeriods = await paymentPeriodService.GetPaymentPeriods(companyId, requestId, page, pageSize);
            return Ok(paymentPeriods);
        }

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("schedule")]
        public async Task<IActionResult> SchedulePaymentPeriods()
        {
            if (HttpContext.GetServiceName() != "PaymentPeriodSchedulingService") return Forbid();
            SchedulePaymentPeriodsResult result = await paymentPeriodService.SchedulePaymentPeriods();
            return Ok(result);
        }
    }
}