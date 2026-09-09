using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.Payments
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentRepository paymentRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPayments(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? studentId = null,
            [FromQuery] string? receiptReference = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Payment> payments = await paymentRepository.GetPayments(companyId, date, studentId, receiptReference, page, pageSize);
            return Ok(payments);
        }

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetPayment(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            Payment? payment = await paymentRepository.GetPayment(companyId, id);
            return payment == null ? NotFound() : Ok(payment);
        }

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPayment([FromBody] Payment payment)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await paymentRepository.InsertPayment(companyId, payment);
            return id > 0 ? CreatedAtAction(nameof(GetPayment), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdatePayment(long id, [FromBody] Payment payment, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = (await paymentRepository.UpdatePayment(companyId, id, payment)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeletePayment(long id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = (await paymentRepository.DeletePayment(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}