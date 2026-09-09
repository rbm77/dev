using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.EmailAccounts
{
    [Route("email-accounts")]
    [ApiController]
    public class EmailAccountsController(IEmailAccountRepository emailAccountRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EMAIL_ACCOUNT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetEmailAccounts([FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<EmailAccount> emailAccounts = await emailAccountRepository.GetEmailAccounts(companyId, isActive, page, pageSize);
            return Ok(emailAccounts);
        }

        [Authorize(Policy = $"{Resources.EMAIL_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertEmailAccount([FromBody] EmailAccount emailAccount)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await emailAccountRepository.InsertEmailAccount(companyId, emailAccount);
            return id > 0 ? CreatedAtAction(nameof(GetEmailAccounts), null, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EMAIL_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmailAccount(int id, [FromBody] EmailAccount emailAccount)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await emailAccountRepository.UpdateEmailAccount(companyId, id, emailAccount)) > 0;
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
            bool deleted = (await emailAccountRepository.DeleteEmailAccount(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}
