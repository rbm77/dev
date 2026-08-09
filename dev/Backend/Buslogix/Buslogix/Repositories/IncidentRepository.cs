using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Repositories
{
    public class IncidentRepository(IDataAccess dataAccess) : IIncidentRepository
    {

        public async Task<Incident?> GetIncident(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Incident> rows = await dataAccess.ExecuteReader("get_incident", CommandType.StoredProcedure,
                static reader => new Incident
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    DriverId = reader.GetInt32OrDefault(2),
                    Date = reader.GetDateTimeOrDefault(3),
                    Description = reader.GetStringOrDefault(4),
                    Type = (IncidentType)reader.GetInt32OrDefault(5),
                    CorrectiveActions = reader.GetStringOrDefault(6)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<PagedResult<Incident>> GetIncidents(
            int companyId,
            int? vehicleId = null,
            int? driverId = null,
            IncidentType? type = null,
            int page = 1,
            int pageSize = 20
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = vehicleId,
                ["p_driver_id"] = driverId,
                ["p_type"] = type,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<Incident> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_incidents", CommandType.StoredProcedure,
                static reader => new Incident
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    DriverId = reader.GetInt32OrDefault(2),
                    Date = reader.GetDateTimeOrDefault(3),
                    Type = (IncidentType)reader.GetInt32OrDefault(4)
                }, parameters);

            return new PagedResult<Incident>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<int> InsertIncident(int companyId, Incident incident)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = incident.VehicleId,
                ["p_driver_id"] = incident.DriverId,
                ["p_date"] = incident.Date,
                ["p_description"] = incident.Description,
                ["p_type"] = (int)incident.Type,
                ["p_corrective_actions"] = incident.CorrectiveActions
            };

            object? result = await dataAccess.ExecuteScalar("insert_incident", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateIncident(int companyId, int id, Incident incident)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_vehicle_id"] = incident.VehicleId,
                ["p_driver_id"] = incident.DriverId,
                ["p_date"] = incident.Date,
                ["p_description"] = incident.Description,
                ["p_type"] = (int)incident.Type,
                ["p_corrective_actions"] = incident.CorrectiveActions
            };

            return await dataAccess.ExecuteNonQuery("update_incident", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteIncident(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_incident", CommandType.StoredProcedure, parameters);
        }
    }
}