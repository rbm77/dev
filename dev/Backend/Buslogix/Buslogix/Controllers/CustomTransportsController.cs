using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("custom-transports")]
    [ApiController]
    public class CustomTransportsController(ICustomTransportService customTransportService) : ControllerBase
    {

        private readonly ICustomTransportService _customTransportService = customTransportService;

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.READ}")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCustomTransports(
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<CustomTransport> customTransports = await _customTransportService.GetPendingCustomTransports(companyId, vehicleId, driverId);
            return Ok(customTransports);
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.READ}")]
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedCustomTransports(
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<CustomTransport> customTransports = await _customTransportService.GetCompletedCustomTransports(companyId, vehicleId, driverId);
            return Ok(customTransports);
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomTransport(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            CustomTransport? customTransport = await _customTransportService.GetCustomTransport(companyId, id);
            return customTransport == null ? NotFound() : Ok(customTransport);
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertCustomTransport([FromBody] CustomTransport customTransport)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _customTransportService.InsertCustomTransport(companyId, customTransport);
            return id > 0 ? CreatedAtAction(nameof(GetCustomTransport), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomTransport(int id, [FromBody] CustomTransport customTransport)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _customTransportService.UpdateCustomTransport(companyId, id, customTransport);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomTransport(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _customTransportService.DeleteCustomTransport(companyId, id);
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteCustomTransport(int id, [FromBody] CustomTransport customTransport)
        {
            int companyId = HttpContext.GetCompanyId();
            bool completed = await _customTransportService.CompleteCustomTransport(companyId, id, customTransport);
            return completed ? NoContent() : NotFound();
        }
    }
}