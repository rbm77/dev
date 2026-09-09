using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.Expenses
{
    [Route("[controller]")]
    [ApiController]
    public class ExpensesController(IExpenseRepository expenseRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Expense> expenses = await expenseRepository.GetExpenses(companyId, date, page, pageSize);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            Expense? expense = await expenseRepository.GetExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertExpense([FromBody] Expense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await expenseRepository.InsertExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateExpense(long id, [FromBody] Expense expense, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = (await expenseRepository.UpdateExpense(companyId, id, expense)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteExpense(long id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = (await expenseRepository.DeleteExpense(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}