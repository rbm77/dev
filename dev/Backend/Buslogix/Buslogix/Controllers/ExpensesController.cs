using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExpensesController(IExpenseService expenseService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetExpenses(
            [FromQuery] DateTime? date = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Expense> expenses = await expenseService.GetExpenses(companyId, date);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            Expense? expense = await expenseService.GetExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertExpense([FromBody] Expense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await expenseService.InsertExpense(companyId, expense);
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
            bool updated = await expenseService.UpdateExpense(companyId, id, expense);
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
            bool deleted = await expenseService.DeleteExpense(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}