using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("students/{studentId:int}/[controller]")]
    [ApiController]
    public class ContactsController(IContactService contactService) : ControllerBase
    {

        private readonly IContactService _contactService = contactService;

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetContacts(int studentId)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Contact> contacts = await _contactService.GetContacts(companyId, studentId);
            return Ok(contacts);
        }

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertContact(int studentId, [FromBody] Contact contact)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _contactService.InsertContact(companyId, studentId, contact);
            return id > 0 ? CreatedAtAction(nameof(GetContacts), new { studentId }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateContact(int studentId, int id, [FromBody] Contact contact)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _contactService.UpdateContact(companyId, studentId, id, contact);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.CONTACT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteContact(int studentId, int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _contactService.DeleteContact(companyId, studentId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}