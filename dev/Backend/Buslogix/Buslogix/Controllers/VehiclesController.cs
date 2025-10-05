using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VehiclesController(IVehicleService vehicleService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetVehicles(
            [FromQuery] bool? isActive = null,
            [FromQuery] string? licensePlate = null,
            [FromQuery] string? make = null,
            [FromQuery] string? model = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Vehicle> vehicles = await vehicleService.GetVehicles(companyId, isActive, licensePlate, make, model);
            return Ok(vehicles);
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Vehicle? vehicle = await vehicleService.GetVehicle(companyId, id);
            return vehicle == null ? NotFound() : Ok(vehicle);
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertVehicle([FromBody] Vehicle vehicle)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await vehicleService.InsertVehicle(companyId, vehicle);
            return id > 0 ? CreatedAtAction(nameof(GetVehicle), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] Vehicle vehicle)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await vehicleService.UpdateVehicle(companyId, id, vehicle);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.VEHICLE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVehicle(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await vehicleService.DeleteVehicle(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}