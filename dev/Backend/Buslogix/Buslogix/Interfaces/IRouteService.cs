using Route = Buslogix.Models.Route;

namespace Buslogix.Interfaces
{
    public interface IRouteService
    {

        Task<Route?> GetRoute(int companyId, int id);

        Task<List<Route>> GetRoutes(int companyId, bool? isActive = null, string? name = null);

        Task<int> InsertRoute(int companyId, Route route);

        Task<bool> UpdateRoute(int companyId, int id, Route route);

        Task<bool> DeleteRoute(int companyId, int id);
    }
}