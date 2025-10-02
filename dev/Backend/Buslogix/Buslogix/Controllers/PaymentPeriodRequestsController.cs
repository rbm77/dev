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

        private readonly IPaymentPeriodRequestService _paymentPeriodRequestService = paymentPeriodRequestService;

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD_REQUEST}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentPeriodRequests()
        {
            int companyId = HttpContext.GetCompanyId();
            List<PaymentPeriodRequest> requests = await _paymentPeriodRequestService.GetPaymentPeriodRequests(companyId);
            return Ok(requests);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD_REQUEST}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentPeriodRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            PaymentPeriodRequest? request = await _paymentPeriodRequestService.GetPaymentPeriodRequest(companyId, id);
            return request == null ? NotFound() : Ok(request);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD_REQUEST}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPaymentPeriodRequest([FromBody] PaymentPeriodRequest request)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _paymentPeriodRequestService.InsertPaymentPeriodRequest(companyId, request);
            return id > 0 ? CreatedAtAction(nameof(GetPaymentPeriodRequest), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD_REQUEST}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePaymentPeriodRequest(int id, [FromBody] PaymentPeriodRequest request)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _paymentPeriodRequestService.UpdatePaymentPeriodRequest(companyId, id, request);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.PAYMENT_PERIOD_REQUEST}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePaymentPeriodRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _paymentPeriodRequestService.DeletePaymentPeriodRequest(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}