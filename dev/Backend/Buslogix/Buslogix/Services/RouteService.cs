using Buslogix.Interfaces;
using Buslogix.Models;
using Route = Buslogix.Models.Route;

namespace Buslogix.Services
{
    public class RouteService(IRouteRepository routeRepository) : IRouteService
    {

        public async Task<Route?> GetRoute(int companyId, int id)
        {
            return await routeRepository.GetRoute(companyId, id);
        }

        public async Task<PagedResult<Route>> GetRoutes(int companyId, bool? isActive, string? name, int page, int pageSize)
        {
            return await routeRepository.GetRoutes(companyId, isActive, name, page, pageSize);
        }

        public async Task<int> InsertRoute(int companyId, Route route)
        {
            return await routeRepository.InsertRoute(companyId, route);
        }

        public async Task<bool> UpdateRoute(int companyId, int id, Route route)
        {
            int affected = await routeRepository.UpdateRoute(companyId, id, route);
            return affected > 0;
        }

        public async Task<bool> DeleteRoute(int companyId, int id)
        {
            int affected = await routeRepository.DeleteRoute(companyId, id);
            return affected > 0;
        }
    }
}