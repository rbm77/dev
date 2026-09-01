using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("email-accounts")]
    [ApiController]
    public class EmailAccountsController(IEmailAccountService emailAccountService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EMAIL_ACCOUNT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetEmailAccounts([FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<EmailAccount> emailAccounts = await emailAccountService.GetEmailAccounts(companyId, isActive, page, pageSize);
            return Ok(emailAccounts);
        }

        [Authorize(Policy = $"{Resources.EMAIL_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertEmailAccount([FromBody] EmailAccount emailAccount)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await emailAccountService.InsertEmailAccount(companyId, emailAccount);
            return id > 0 ? CreatedAtAction(nameof(GetEmailAccounts), null, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EMAIL_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmailAccount(int id, [FromBody] EmailAccount emailAccount)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await emailAccountService.UpdateEmailAccount(companyId, id, emailAccount);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.EMAIL_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmailAccount(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await emailAccountService.DeleteEmailAccount(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
