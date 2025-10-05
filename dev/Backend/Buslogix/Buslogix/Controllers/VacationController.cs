using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VacationController(IVacationService vacationService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetAllVacation()
        {
            int companyId = HttpContext.GetCompanyId();
            List<Vacation> vacations = await vacationService.GetAllVacation(companyId);
            return Ok(vacations);
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVacation(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Vacation? vacation = await vacationService.GetVacation(companyId, id);
            return vacation == null ? NotFound() : Ok(vacation);
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertVacation([FromBody] Vacation vacation)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await vacationService.InsertVacation(companyId, vacation);
            return id > 0 ? CreatedAtAction(nameof(GetVacation), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVacation(int id, [FromBody] Vacation vacation, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await vacationService.UpdateVacation(companyId, id, vacation);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVacation(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await vacationService.DeleteVacation(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}