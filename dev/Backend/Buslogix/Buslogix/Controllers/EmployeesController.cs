using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeeService employeeService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] bool? isActive = null,
            [FromQuery] string? identityDocument = null,
            [FromQuery] string? name = null,
            [FromQuery] string? lastName = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Employee> employees = await employeeService.GetEmployees(
                companyId, isActive, identityDocument, name, lastName);
            return Ok(employees);
        }

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Employee? employee = await employeeService.GetEmployee(companyId, id);
            return employee == null ? NotFound() : Ok(employee);
        }

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertEmployee([FromBody] Employee employee)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await employeeService.InsertEmployee(companyId, employee);
            return id > 0 ? CreatedAtAction(nameof(GetEmployee), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await employeeService.UpdateEmployee(companyId, id, employee);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await employeeService.DeleteEmployee(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
