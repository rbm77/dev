using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.Employees
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeeRepository employeeRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] bool? isActive = null,
            [FromQuery] string? identityDocument = null,
            [FromQuery] string? name = null,
            [FromQuery] string? lastName = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Employee> employees = await employeeRepository.GetEmployees(
                companyId, isActive, identityDocument, name, lastName, page, pageSize);
            return Ok(employees);
        }

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Employee? employee = await employeeRepository.GetEmployee(companyId, id);
            return employee == null ? NotFound() : Ok(employee);
        }

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertEmployee([FromBody] Employee employee)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await employeeRepository.InsertEmployee(companyId, employee);
            return id > 0 ? CreatedAtAction(nameof(GetEmployee), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EMPLOYEE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await employeeRepository.UpdateEmployee(companyId, id, employee)) > 0;
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
            bool deleted = (await employeeRepository.DeleteEmployee(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}
