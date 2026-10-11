using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Salaries
{
    [ApiController]
    [Route("employees/{employeeId:int}/[controller]")]
    public class SalariesController(ISalaryRepository salaryRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetSalaries(int employeeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Salary> salaries = await salaryRepository.GetSalaries(companyId, employeeId, page, pageSize);
            return Ok(salaries);
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertSalary(int employeeId, [FromBody] Salary salary)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await salaryRepository.InsertSalary(companyId, employeeId, salary);
            return id > 0 ? CreatedAtAction(nameof(GetSalaries), new { employeeId }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSalary(int employeeId, int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting Salary {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await salaryRepository.DeleteSalary(companyId, employeeId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}