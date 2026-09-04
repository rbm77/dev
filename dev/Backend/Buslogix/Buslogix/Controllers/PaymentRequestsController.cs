using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Triggers.Queues;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("payment-requests")]
    [ApiController]
    public class PaymentRequestsController(
        IPaymentRequestService paymentRequestService,
        PaymentAutoApprovalQueue paymentAutoApprovalQueue,
        PaymentMatchSweepQueue paymentMatchSweepQueue) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PAYMENT_REQUEST}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentRequests(
            [FromQuery] DateTime? requestedAt = null,
            [FromQuery] bool? isValidated = null,
            [FromQuery] string? receiptReference = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<PaymentRequest> paymentRequests = await paymentRequestService.GetPaymentRequests(companyId, requestedAt, isValidated, receiptReference, page, pageSize);
            return Ok(paymentRequests);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_REQUEST}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            PaymentRequest? paymentRequest = await paymentRequestService.GetPaymentRequest(companyId, id);
            return paymentRequest == null ? NotFound() : Ok(paymentRequest);
        }

        [Authorize(Policy = $"{Resources.PAYMENT_REQUEST}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPaymentRequest([FromBody] PaymentRequest paymentRequest)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await paymentRequestService.InsertPaymentRequest(companyId, paymentRequest);
            return id > 0 ? CreatedAtAction(nameof(GetPaymentRequest), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.PAYMENT_APPROVAL}.{PermissionMode.WRITE}")]
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> ApprovePaymentRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            long newPaymentId = await paymentRequestService.ApprovePaymentRequest(companyId, id);
            return newPaymentId > 0
                ? CreatedAtAction("GetPayment", "Payments", new { id = newPaymentId }, null)
                : NotFound();
        }

        [Authorize(Policy = $"{Resources.PAYMENT_APPROVAL}.{PermissionMode.WRITE}")]
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> RejectPaymentRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool rejected = await paymentRequestService.RejectPaymentRequest(companyId, id);
            return rejected ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.PAYMENT_APPROVAL}.{PermissionMode.WRITE}")]
        [HttpPost("{id:int}/validate")]
        public async Task<IActionResult> ValidatePaymentRequest(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool validated = await paymentRequestService.ValidatePaymentRequest(companyId, id);
            return validated ? NoContent() : NotFound();
        }

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("auto-approve")]
        public IActionResult AutoApprovePaymentRequests()
        {
            if (HttpContext.GetServiceName() != "PaymentAutoApprovalService") return Forbid();
            paymentAutoApprovalQueue.TryTrigger();
            return Accepted();
        }

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("match-pending")]
        public IActionResult MatchPendingPaymentRequests()
        {
            if (HttpContext.GetServiceName() != "PaymentMatchingService") return Forbid();
            paymentMatchSweepQueue.TryTrigger();
            return Accepted();
        }
    }
}
