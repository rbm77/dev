using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Vacations
{
    [Route("[controller]")]
    [ApiController]
    public class VacationController(IVacationRepository vacationRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetAllVacation([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Vacation> vacations = await vacationRepository.GetAllVacation(companyId, page, pageSize);
            return Ok(vacations);
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVacation(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Vacation? vacation = await vacationRepository.GetVacation(companyId, id);
            return vacation == null ? NotFound() : Ok(vacation);
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertVacation([FromBody] Vacation vacation)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await vacationRepository.InsertVacation(companyId, vacation);
            return id > 0 ? CreatedAtAction(nameof(GetVacation), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVacation(int id, [FromBody] Vacation vacation, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: updating Vacation {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool updated = (await vacationRepository.UpdateVacation(companyId, id, vacation)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.VACATION}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVacation(int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting Vacation {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await vacationRepository.DeleteVacation(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}