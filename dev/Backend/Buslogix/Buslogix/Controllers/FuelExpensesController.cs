using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("fuel-expenses")]
    [ApiController]
    public class FuelExpensesController(IFuelExpenseService fuelExpenseService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetFuelExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<FuelExpense> expenses = await fuelExpenseService.GetFuelExpenses(companyId, date, vehicleId, driverId);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetFuelExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            FuelExpense? expense = await fuelExpenseService.GetFuelExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertFuelExpense([FromBody] FuelExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await fuelExpenseService.InsertFuelExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetFuelExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateFuelExpense(long id, [FromBody] FuelExpense expense, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await fuelExpenseService.UpdateFuelExpense(companyId, id, expense);
            return updated ? NoContent() : NotFound();
        }
    }
}