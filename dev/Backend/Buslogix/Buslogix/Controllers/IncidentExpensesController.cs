using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("incident-expenses")]
    [ApiController]
    public class IncidentExpensesController(IIncidentExpenseService incidentExpenseService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetIncidentExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? incidentId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<IncidentExpense> expenses = await incidentExpenseService.GetIncidentExpenses(companyId, date, incidentId, page, pageSize);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetIncidentExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            IncidentExpense? expense = await incidentExpenseService.GetIncidentExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertIncidentExpense([FromBody] IncidentExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await incidentExpenseService.InsertIncidentExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetIncidentExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateIncidentExpense(long id, [FromBody] IncidentExpense expense, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await incidentExpenseService.UpdateIncidentExpense(companyId, id, expense);
            return updated ? NoContent() : NotFound();
        }
    }
}
