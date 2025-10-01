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

        private readonly IIncidentExpenseService _incidentExpenseService = incidentExpenseService;

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetIncidentExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? incidentId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<IncidentExpense> expenses = await _incidentExpenseService.GetIncidentExpenses(companyId, date, incidentId);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetIncidentExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            IncidentExpense? expense = await _incidentExpenseService.GetIncidentExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertIncidentExpense([FromBody] IncidentExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await _incidentExpenseService.InsertIncidentExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetIncidentExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateIncidentExpense(long id, [FromBody] IncidentExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _incidentExpenseService.UpdateIncidentExpense(companyId, id, expense);
            return updated ? NoContent() : NotFound();
        }
    }
}
