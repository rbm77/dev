using Buslogix.Interfaces;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Route = Buslogix.Models.Route;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoutesController(IRouteService routeService) : ControllerBase
    {

        private readonly IRouteService _routeService = routeService;

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetRoutes(
            [FromQuery] bool? isActive = null,
            [FromQuery] string? name = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Route> routes = await _routeService.GetRoutes(companyId, isActive, name);
            return Ok(routes);
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoute(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Route? route = await _routeService.GetRoute(companyId, id);
            return route == null ? NotFound() : Ok(route);
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertRoute([FromBody] Route route)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _routeService.InsertRoute(companyId, route);
            return id > 0 ? CreatedAtAction(nameof(GetRoute), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] Route route)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _routeService.UpdateRoute(companyId, id, route);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.ROUTE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _routeService.DeleteRoute(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}