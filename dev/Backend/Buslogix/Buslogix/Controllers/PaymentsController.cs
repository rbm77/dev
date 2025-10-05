using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentService paymentService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetPayments(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? studentId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Payment> payments = await paymentService.GetPayments(companyId, date, studentId);
            return Ok(payments);
        }

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetPayment(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            Payment? payment = await paymentService.GetPayment(companyId, id);
            return payment == null ? NotFound() : Ok(payment);
        }

        [Authorize(Policy = $"{Resources.PAYMENT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertPayment([FromBody] Payment payment)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await paymentService.InsertPayment(companyId, payment);
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
            bool updated = await paymentService.UpdatePayment(companyId, id, payment);
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
            bool deleted = await paymentService.DeletePayment(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}