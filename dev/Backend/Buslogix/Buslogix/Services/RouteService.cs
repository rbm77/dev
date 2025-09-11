using Buslogix.Interfaces;
using Route = Buslogix.Models.Route;

namespace Buslogix.Services
{
    public class RouteService(IRouteRepository routeRepository) : IRouteService
    {

        private readonly IRouteRepository _routeRepository = routeRepository;

        public async Task<Route?> GetRoute(int companyId, int id)
        {
            return await _routeRepository.GetRoute(companyId, id);
        }

        public async Task<List<Route>> GetRoutes(int companyId, bool? isActive = null, string? name = null)
        {
            return await _routeRepository.GetRoutes(companyId, isActive, name);
        }

        public async Task<int> InsertRoute(int companyId, Route route)
        {
            return await _routeRepository.InsertRoute(companyId, route);
        }

        public async Task<bool> UpdateRoute(int companyId, int id, Route route)
        {
            int affected = await _routeRepository.UpdateRoute(companyId, id, route);
            return affected > 0;
        }

        public async Task<bool> DeleteRoute(int companyId, int id)
        {
            int affected = await _routeRepository.DeleteRoute(companyId, id);
            return affected > 0;
        }
    }
}