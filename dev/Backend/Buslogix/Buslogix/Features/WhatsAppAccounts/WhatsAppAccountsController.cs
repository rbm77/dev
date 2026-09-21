using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.WhatsAppAccounts
{
    [Route("whatsapp-accounts")]
    [ApiController]
    public class WhatsAppAccountsController(IWhatsAppAccountRepository whatsAppAccountRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.WHATSAPP_ACCOUNT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetWhatsAppAccounts([FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<WhatsAppAccount> whatsAppAccounts = await whatsAppAccountRepository.GetWhatsAppAccounts(companyId, isActive, page, pageSize);

            PagedResult<WhatsAppAccountSummary> response = new()
            {
                Items = whatsAppAccounts.Items.Select(WhatsAppAccountSummary.FromAccount).ToList(),
                TotalCount = whatsAppAccounts.TotalCount,
                Page = whatsAppAccounts.Page,
                PageSize = whatsAppAccounts.PageSize
            };

            return Ok(response);
        }

        [Authorize(Policy = $"{Resources.WHATSAPP_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertWhatsAppAccount([FromBody] WhatsAppAccount whatsAppAccount)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await whatsAppAccountRepository.InsertWhatsAppAccount(companyId, whatsAppAccount);
            return id > 0 ? CreatedAtAction(nameof(GetWhatsAppAccounts), null, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.WHATSAPP_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWhatsAppAccount(int id, [FromBody] WhatsAppAccount whatsAppAccount)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await whatsAppAccountRepository.UpdateWhatsAppAccount(companyId, id, whatsAppAccount)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.WHATSAPP_ACCOUNT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteWhatsAppAccount(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = (await whatsAppAccountRepository.DeleteWhatsAppAccount(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}
