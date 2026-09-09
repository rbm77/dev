using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.PaymentPeriods
{
    [Route("payment-periods")]
    [ApiController]
    public class PaymentPeriodsController(
        IPaymentPeriodRepository paymentPeriodRepository,
        PaymentPeriodScheduleQueue paymentPeriodScheduleQueue) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentPeriods(
            [FromQuery] int? requestId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<PaymentPeriod> paymentPeriods = await paymentPeriodRepository.GetPaymentPeriods(companyId, requestId, page, pageSize);
            return Ok(paymentPeriods);
        }

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("schedule")]
        public IActionResult SchedulePaymentPeriods()
        {
            if (HttpContext.GetServiceName() != "PaymentPeriodSchedulingService") return Forbid();
            paymentPeriodScheduleQueue.TryTrigger();
            return Accepted();
        }
    }
}