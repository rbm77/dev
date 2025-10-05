using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [ApiController]
    [Route("employees/{employeeId:int}/[controller]")]
    public class SalariesController(ISalaryService salaryService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetSalaries(int employeeId)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Salary> salaries = await salaryService.GetSalaries(companyId, employeeId);
            return Ok(salaries);
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertSalary(int employeeId, [FromBody] Salary salary)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await salaryService.InsertSalary(companyId, employeeId, salary);
            return id > 0 ? CreatedAtAction(nameof(GetSalaries), new { employeeId }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSalary(int employeeId, int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await salaryService.DeleteSalary(companyId, employeeId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}