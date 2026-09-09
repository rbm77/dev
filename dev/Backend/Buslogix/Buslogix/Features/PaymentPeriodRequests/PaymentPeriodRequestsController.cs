using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.PaymentPeriodRequests
{
    [Route("payment-period-requests")]
    [ApiController]
    public class PaymentPeriodRequestsController(IPaymentPeriodRequestRepository paymentPeriodRequestRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentPeriodRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<PaymentPeriodRequest> requests = await paymentPeriodRequestRepository.GetPaymentPeriodRequests(companyId, page, pageSize);
            return Ok(requests);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentPeriodRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            PaymentPeriodRequest? request = await paymentPeriodRequestRepository.GetPaymentPeriodRequest(companyId, id);
            return request == null ? NotFound() : Ok(request);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPaymentPeriodRequest([FromBody] PaymentPeriodRequest request)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await paymentPeriodRequestRepository.InsertPaymentPeriodRequest(companyId, request);
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
            bool updated = (await paymentPeriodRequestRepository.UpdatePaymentPeriodRequest(companyId, id, request)) > 0;
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
            bool deleted = (await paymentPeriodRequestRepository.DeletePaymentPeriodRequest(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}