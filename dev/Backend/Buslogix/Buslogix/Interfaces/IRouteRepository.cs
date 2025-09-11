using Route = Buslogix.Models.Route;

namespace Buslogix.Interfaces
{
    public interface IRouteRepository
    {

        Task<Route?> GetRoute(int companyId, int id);

        Task<List<Route>> GetRoutes(int companyId, bool? isActive = null, string? name = null);

        Task<int> InsertRoute(int companyId, Route route);

        Task<int> UpdateRoute(int companyId, int id, Route route);

        Task<int> DeleteRoute(int companyId, int id);
    }
}
