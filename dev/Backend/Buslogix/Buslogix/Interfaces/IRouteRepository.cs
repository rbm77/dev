using Buslogix.Models;
using Route = Buslogix.Models.Route;

namespace Buslogix.Interfaces
{
    public interface IRouteRepository
    {

        Task<Route?> GetRoute(int companyId, int id);

        Task<PagedResult<Route>> GetRoutes(int companyId, bool? isActive, string? name, int page, int pageSize);

        Task<int> InsertRoute(int companyId, Route route);

        Task<int> UpdateRoute(int companyId, int id, Route route);

        Task<int> DeleteRoute(int companyId, int id);
    }
}
