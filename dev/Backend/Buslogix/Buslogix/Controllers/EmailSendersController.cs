using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("email-senders")]
    [ApiController]
    public class EmailSendersController(IEmailSenderService emailSenderService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EMAIL_SENDER}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetEmailSenders([FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<EmailSender> emailSenders = await emailSenderService.GetEmailSenders(companyId, isActive, page, pageSize);
            return Ok(emailSenders);
        }

        [Authorize(Policy = $"{Resources.EMAIL_SENDER}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertEmailSender([FromBody] EmailSender emailSender)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await emailSenderService.InsertEmailSender(companyId, emailSender);
            return id > 0 ? CreatedAtAction(nameof(GetEmailSenders), null, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EMAIL_SENDER}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmailSender(int id, [FromBody] EmailSender emailSender)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await emailSenderService.UpdateEmailSender(companyId, id, emailSender);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.EMAIL_SENDER}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmailSender(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await emailSenderService.DeleteEmailSender(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
