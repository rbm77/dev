using Buslogix.Models;
using Route = Buslogix.Models.Route;

namespace Buslogix.Interfaces
{
    public interface IRouteService
    {

        Task<Route?> GetRoute(int companyId, int id);

        Task<PagedResult<Route>> GetRoutes(int companyId, bool? isActive, string? name, int page, int pageSize);

        Task<int> InsertRoute(int companyId, Route route);

        Task<bool> UpdateRoute(int companyId, int id, Route route);

        Task<bool> DeleteRoute(int companyId, int id);
    }
}