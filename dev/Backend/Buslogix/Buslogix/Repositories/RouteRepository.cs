using System.Data;
using Buslogix.Interfaces;
using Buslogix.Utilities;
using Route = Buslogix.Models.Route;

namespace Buslogix.Repositories
{
    public class RouteRepository(IDataAccess dataAccess) : IRouteRepository
    {

        public async Task<Route?> GetRoute(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Route> rows = await dataAccess.ExecuteReader("get_route", CommandType.StoredProcedure,
                static reader => new Route
                {
                    Id = reader.GetInt32OrDefault(0),
                    Name = reader.GetStringOrDefault(1),
                    Description = reader.GetStringOrDefault(2),
                    IsActive = reader.GetBooleanOrDefault(3)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Route>> GetRoutes(int companyId, bool? isActive = null, string? name = null)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_is_active"] = isActive,
                ["p_name"] = name
            };

            List<Route> rows = await dataAccess.ExecuteReader("get_routes", CommandType.StoredProcedure,
                static reader => new Route
                {
                    Id = reader.GetInt32OrDefault(0),
                    Name = reader.GetStringOrDefault(1),
                    Description = reader.GetStringOrDefault(2),
                    IsActive = reader.GetBooleanOrDefault(3)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertRoute(int companyId, Route route)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_name"] = route.Name,
                ["p_description"] = route.Description,
                ["p_is_active"] = route.IsActive
            };

            object? result = await dataAccess.ExecuteScalar("insert_route", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateRoute(int companyId, int id, Route route)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_name"] = route.Name,
                ["p_description"] = route.Description,
                ["p_is_active"] = route.IsActive
            };

            return await dataAccess.ExecuteNonQuery("update_route", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteRoute(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_route", CommandType.StoredProcedure, parameters);
        }
    }
}