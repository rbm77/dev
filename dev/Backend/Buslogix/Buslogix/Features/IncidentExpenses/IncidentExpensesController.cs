using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.IncidentExpenses
{
    [Route("incident-expenses")]
    [ApiController]
    public class IncidentExpensesController(IIncidentExpenseRepository incidentExpenseRepository) : ControllerBase
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
            PagedResult<IncidentExpense> expenses = await incidentExpenseRepository.GetIncidentExpenses(companyId, date, incidentId, page, pageSize);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetIncidentExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            IncidentExpense? expense = await incidentExpenseRepository.GetIncidentExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertIncidentExpense([FromBody] IncidentExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await incidentExpenseRepository.InsertIncidentExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetIncidentExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateIncidentExpense(long id, [FromBody] IncidentExpense expense, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: updating IncidentExpense {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool updated = (await incidentExpenseRepository.UpdateIncidentExpense(companyId, id, expense)) > 0;
            return updated ? NoContent() : NotFound();
        }
    }
}
