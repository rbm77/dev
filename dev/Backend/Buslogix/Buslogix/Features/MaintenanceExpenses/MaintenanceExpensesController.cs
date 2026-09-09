using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.MaintenanceExpenses
{
    [Route("maintenance-expenses")]
    [ApiController]
    public class MaintenanceExpensesController(IMaintenanceExpenseRepository maintenanceExpenseRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetMaintenanceExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? maintenanceId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<MaintenanceExpense> expenses = await maintenanceExpenseRepository.GetMaintenanceExpenses(companyId, date, maintenanceId, page, pageSize);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetMaintenanceExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            MaintenanceExpense? expense = await maintenanceExpenseRepository.GetMaintenanceExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertMaintenanceExpense([FromBody] MaintenanceExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await maintenanceExpenseRepository.InsertMaintenanceExpense(companyId, expense);
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
            bool updated = (await maintenanceExpenseRepository.UpdateMaintenanceExpense(companyId, id, expense)) > 0;
            return updated ? NoContent() : NotFound();
        }
    }
}