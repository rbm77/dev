using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.SalaryExpenses
{
    [Route("salary-expenses")]
    [ApiController]
    public class SalaryExpensesController(ISalaryExpenseRepository salaryExpenseRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetSalaryExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? employeeId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<SalaryExpense> expenses = await salaryExpenseRepository.GetSalaryExpenses(companyId, date, employeeId, page, pageSize);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetSalaryExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            SalaryExpense? expense = await salaryExpenseRepository.GetSalaryExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertSalaryExpense([FromBody] SalaryExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await salaryExpenseRepository.InsertSalaryExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetSalaryExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateSalaryExpense(long id, [FromBody] SalaryExpense expense, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: updating SalaryExpense {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool updated = (await salaryExpenseRepository.UpdateSalaryExpense(companyId, id, expense)) > 0;
            return updated ? NoContent() : NotFound();
        }
    }
}