using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.FuelExpenses
{
    [Route("fuel-expenses")]
    [ApiController]
    public class FuelExpensesController(IFuelExpenseRepository fuelExpenseRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetFuelExpenses(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<FuelExpense> expenses = await fuelExpenseRepository.GetFuelExpenses(companyId, date, vehicleId, driverId, page, pageSize);
            return Ok(expenses);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.READ}")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetFuelExpense(long id)
        {
            int companyId = HttpContext.GetCompanyId();
            FuelExpense? expense = await fuelExpenseRepository.GetFuelExpense(companyId, id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertFuelExpense([FromBody] FuelExpense expense)
        {
            int companyId = HttpContext.GetCompanyId();
            long id = await fuelExpenseRepository.InsertFuelExpense(companyId, expense);
            return id > 0 ? CreatedAtAction(nameof(GetFuelExpense), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.EXPENSE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateFuelExpense(long id, [FromBody] FuelExpense expense, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: updating FuelExpense {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool updated = (await fuelExpenseRepository.UpdateFuelExpense(companyId, id, expense)) > 0;
            return updated ? NoContent() : NotFound();
        }
    }
}