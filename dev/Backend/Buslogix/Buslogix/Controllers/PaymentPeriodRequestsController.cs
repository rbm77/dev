using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("payment-period-requests")]
    [ApiController]
    public class PaymentPeriodRequestsController(IPaymentPeriodRequestService paymentPeriodRequestService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentPeriodRequests()
        {
            int companyId = HttpContext.GetCompanyId();
            List<PaymentPeriodRequest> requests = await paymentPeriodRequestService.GetPaymentPeriodRequests(companyId);
            return Ok(requests);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentPeriodRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            PaymentPeriodRequest? request = await paymentPeriodRequestService.GetPaymentPeriodRequest(companyId, id);
            return request == null ? NotFound() : Ok(request);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPaymentPeriodRequest([FromBody] PaymentPeriodRequest request)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await paymentPeriodRequestService.InsertPaymentPeriodRequest(companyId, request);
            return id > 0 ? CreatedAtAction(nameof(GetPaymentPeriodRequest), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePaymentPeriodRequest(int id, [FromBody] PaymentPeriodRequest request, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await paymentPeriodRequestService.UpdatePaymentPeriodRequest(companyId, id, request);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePaymentPeriodRequest(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await paymentPeriodRequestService.DeletePaymentPeriodRequest(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}