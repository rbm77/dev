using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.CustomTransports
{
    [Route("custom-transports")]
    [ApiController]
    public class CustomTransportsController(ICustomTransportRepository customTransportRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.READ}")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCustomTransports(
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<CustomTransport> customTransports = await customTransportRepository.GetPendingCustomTransports(companyId, vehicleId, driverId, page, pageSize);
            return Ok(customTransports);
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.READ}")]
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedCustomTransports(
            [FromQuery] int? vehicleId = null,
            [FromQuery] int? driverId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<CustomTransport> customTransports = await customTransportRepository.GetCompletedCustomTransports(companyId, vehicleId, driverId, page, pageSize);
            return Ok(customTransports);
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomTransport(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            CustomTransport? customTransport = await customTransportRepository.GetCustomTransport(companyId, id);
            return customTransport == null ? NotFound() : Ok(customTransport);
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertCustomTransport([FromBody] CustomTransport customTransport)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await customTransportRepository.InsertCustomTransport(companyId, customTransport);
            return id > 0 ? CreatedAtAction(nameof(GetCustomTransport), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomTransport(int id, [FromBody] CustomTransport customTransport, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = (await customTransportRepository.UpdateCustomTransport(companyId, id, customTransport)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomTransport(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = (await customTransportRepository.DeleteCustomTransport(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.CUSTOM_TRANSPORT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteCustomTransport(int id, [FromBody] CustomTransport customTransport)
        {
            int companyId = HttpContext.GetCompanyId();
            bool completed = (await customTransportRepository.CompleteCustomTransport(companyId, id, customTransport)) > 0;
            return completed ? NoContent() : NotFound();
        }
    }
}