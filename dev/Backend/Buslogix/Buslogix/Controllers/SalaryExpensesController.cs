using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("salary-expenses")]
    [ApiController]
    public class SalaryExpensesController(ISalaryExpenseService salaryExpenseService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetSalaryExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? employeeId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<SalaryExpense> expenses = await salaryExpenseService.GetSalaryExpenses(companyId, date, employeeId);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetSalaryExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            SalaryExpense? expense = await salaryExpenseService.GetSalaryExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertSalaryExpense([FromBody] SalaryExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await salaryExpenseService.InsertSalaryExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetSalaryExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateSalaryExpense(long id, [FromBody] SalaryExpense expense, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await salaryExpenseService.UpdateSalaryExpense(companyId, id, expense);
            return updated ? NoContent() : NotFound();
        }
    }
}