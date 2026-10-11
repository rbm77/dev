using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Route = Buslogix.Features.Routes.Route;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Routes
{
    [Route("[controller]")]
    [ApiController]
    public class RoutesController(IRouteRepository routeRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetRoutes(
            [FromQuery] bool? isActive = null,
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Route> routes = await routeRepository.GetRoutes(companyId, isActive, name, page, pageSize);
            return Ok(routes);
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoute(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Route? route = await routeRepository.GetRoute(companyId, id);
            return route == null ? NotFound() : Ok(route);
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertRoute([FromBody] Route route)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await routeRepository.InsertRoute(companyId, route);
            return id > 0 ? CreatedAtAction(nameof(GetRoute), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] Route route)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await routeRepository.UpdateRoute(companyId, id, route)) > 0;
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRoute(int id, [FromServices] IUserService userService, [FromServices] ILogHandler logHandler)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            await logHandler.WriteLog($"Critical process: deleting Route {id} by user {HttpContext.GetUserId()} (company {companyId}).", LogType.Warning);
            bool deleted = (await routeRepository.DeleteRoute(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}