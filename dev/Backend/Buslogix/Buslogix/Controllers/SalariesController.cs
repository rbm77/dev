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

        private readonly ISalaryService _salaryService = salaryService;

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetSalaries(int employeeId)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Salary> salaries = await _salaryService.GetSalaries(companyId, employeeId);
            return Ok(salaries);
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertSalary(int employeeId, [FromBody] Salary salary)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _salaryService.InsertSalary(companyId, employeeId, salary);
            return id > 0 ? CreatedAtAction(nameof(GetSalaries), new { employeeId }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSalary(int employeeId, int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _salaryService.DeleteSalary(companyId, employeeId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}