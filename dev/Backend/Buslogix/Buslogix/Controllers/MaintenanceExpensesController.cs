using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("maintenance-expenses")]
    [ApiController]
    public class MaintenanceExpensesController(IMaintenanceExpenseService maintenanceExpenseService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetMaintenanceExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? maintenanceId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<MaintenanceExpense> expenses = await maintenanceExpenseService.GetMaintenanceExpenses(companyId, date, maintenanceId);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetMaintenanceExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            MaintenanceExpense? expense = await maintenanceExpenseService.GetMaintenanceExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertMaintenanceExpense([FromBody] MaintenanceExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await maintenanceExpenseService.InsertMaintenanceExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetMaintenanceExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateMaintenanceExpense(long id, [FromBody] MaintenanceExpense expense, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await maintenanceExpenseService.UpdateMaintenanceExpense(companyId, id, expense);
            return updated ? NoContent() : NotFound();
        }
    }
}