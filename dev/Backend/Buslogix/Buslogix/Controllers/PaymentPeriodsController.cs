using Buslogix.Interfaces;
using Buslogix.Models;
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
        public async Task<IActionResult> GetPaymentPeriods([FromQuery] int? requestId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<PaymentPeriod> paymentPeriods = await paymentPeriodService.GetPaymentPeriods(companyId, requestId);
            return Ok(paymentPeriods);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SchedulePaymentPeriod([FromHeader(Name = "Cia-Tkn")] string? companyToken)
        {
            if (string.IsNullOrEmpty(companyToken)) return BadRequest();
            PaymentPeriod? paymentPeriod = await paymentPeriodService.SchedulePaymentPeriod(companyToken);
            return paymentPeriod?.Id <= 0 ? BadRequest() : Ok(paymentPeriod);
        }
    }
}