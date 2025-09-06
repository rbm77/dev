using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [ApiController]
    [Route("employees/{employeeId:int}/extra-salaries")]
    public class ExtraSalariesController(IExtraSalaryService extraSalaryService) : ControllerBase
    {

        private readonly IExtraSalaryService _extraSalaryService = extraSalaryService;

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetExtraSalaries(int employeeId)
        {
            int companyId = HttpContext.GetCompanyId();
            List<ExtraSalary> extraSalaries = await _extraSalaryService.GetExtraSalaries(companyId, employeeId);
            return Ok(extraSalaries);
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertExtraSalary(int employeeId, [FromBody] ExtraSalary extraSalary)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _extraSalaryService.InsertExtraSalary(companyId, employeeId, extraSalary);
            return id > 0 ? CreatedAtAction(nameof(GetExtraSalaries), new { employeeId }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.SALARY}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteExtraSalary(int employeeId, int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _extraSalaryService.DeleteExtraSalary(companyId, employeeId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}