using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Contacts
{
    [Route("students/{studentId:int}/[controller]")]
    [ApiController]
    public class ContactsController(IContactRepository contactRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetContacts(int studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Contact> contacts = await contactRepository.GetContacts(companyId, studentId, page, pageSize);
            return Ok(contacts);
        }

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertContact(int studentId, [FromBody] Contact contact)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await contactRepository.InsertContact(companyId, studentId, contact);
            return id > 0 ? CreatedAtAction(nameof(GetContacts), new { studentId }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateContact(int studentId, int id, [FromBody] Contact contact)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await contactRepository.UpdateContact(companyId, studentId, id, contact)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteContact(int studentId, int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting Contact {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await contactRepository.DeleteContact(companyId, studentId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}