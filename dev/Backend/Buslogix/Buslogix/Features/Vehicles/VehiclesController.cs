using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Vehicles
{
    [Route("[controller]")]
    [ApiController]
    public class VehiclesController(IVehicleRepository vehicleRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetVehicles(
            [FromQuery] bool? isActive = null,
            [FromQuery] string? licensePlate = null,
            [FromQuery] string? make = null,
            [FromQuery] string? model = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Vehicle> vehicles = await vehicleRepository.GetVehicles(companyId, isActive, licensePlate, make, model, page, pageSize);
            return Ok(vehicles);
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Vehicle? vehicle = await vehicleRepository.GetVehicle(companyId, id);
            return vehicle == null ? NotFound() : Ok(vehicle);
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertVehicle([FromBody] Vehicle vehicle)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await vehicleRepository.InsertVehicle(companyId, vehicle);
            return id > 0 ? CreatedAtAction(nameof(GetVehicle), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] Vehicle vehicle)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await vehicleRepository.UpdateVehicle(companyId, id, vehicle)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVehicle(int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting Vehicle {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await vehicleRepository.DeleteVehicle(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}