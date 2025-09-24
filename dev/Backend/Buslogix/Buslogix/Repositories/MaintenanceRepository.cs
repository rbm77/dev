using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Repositories
{
    public class MaintenanceRepository(IDataAccess dataAccess) : IMaintenanceRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<Maintenance?> GetMaintenance(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Maintenance> rows = await _dataAccess.ExecuteReader("get_maintenance", CommandType.StoredProcedure,
                static reader => new Maintenance
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    Description = reader.GetStringOrDefault(2),
                    Type = (MaintenanceType)reader.GetInt32OrDefault(3),
                    ScheduledDate = reader.GetDateTimeOrDefault(4),
                    CompletedDate = reader.GetDateTimeOrDefault(5)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Maintenance>> GetPendingMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = vehicleId,
                ["p_type"] = type.HasValue ? (int?)type.Value : null
            };

            List<Maintenance> rows = await _dataAccess.ExecuteReader("get_pending_maintenances", CommandType.StoredProcedure,
                static reader => new Maintenance
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    Type = (MaintenanceType)reader.GetInt32OrDefault(2),
                    ScheduledDate = reader.GetDateTimeOrDefault(3)
                }, parameters);

            return rows;
        }

        public async Task<List<Maintenance>> GetCompletedMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = vehicleId,
                ["p_type"] = type.HasValue ? (int?)type.Value : null
            };

            List<Maintenance> rows = await _dataAccess.ExecuteReader("get_completed_maintenances", CommandType.StoredProcedure,
                static reader => new Maintenance
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    Type = (MaintenanceType)reader.GetInt32OrDefault(2),
                    CompletedDate = reader.GetDateTimeOrDefault(3)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertMaintenance(int companyId, Maintenance maintenance)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = maintenance.VehicleId,
                ["p_description"] = maintenance.Description,
                ["p_type"] = (int)maintenance.Type,
                ["p_scheduled_date"] = maintenance.ScheduledDate
            };

            object? result = await _dataAccess.ExecuteScalar("insert_maintenance", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateMaintenance(int companyId, int id, Maintenance maintenance)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_vehicle_id"] = maintenance.VehicleId,
                ["p_description"] = maintenance.Description,
                ["p_type"] = (int)maintenance.Type,
                ["p_scheduled_date"] = maintenance.ScheduledDate
            };

            return await _dataAccess.ExecuteNonQuery("update_maintenance", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteMaintenance(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await _dataAccess.ExecuteNonQuery("delete_maintenance", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> CompleteMaintenance(int companyId, int id, Maintenance maintenance)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_completed_date"] = maintenance.CompletedDate
            };

            return await _dataAccess.ExecuteNonQuery("complete_maintenance", CommandType.StoredProcedure, parameters);
        }
    }
}