using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class CustomTransportRepository(IDataAccess dataAccess) : ICustomTransportRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<CustomTransport?> GetCustomTransport(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<CustomTransport> rows = await _dataAccess.ExecuteReader("get_custom_transport", CommandType.StoredProcedure,
                static reader => new CustomTransport
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    DriverId = reader.GetInt32OrDefault(2),
                    Amount = reader.GetDecimalOrDefault(3),
                    Description = reader.GetStringOrDefault(4),
                    ScheduledDate = reader.GetDateTimeOrDefault(5),
                    CompletedDate = reader.GetDateTimeOrDefault(6)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<CustomTransport>> GetPendingCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = vehicleId,
                ["p_driver_id"] = driverId
            };

            List<CustomTransport> rows = await _dataAccess.ExecuteReader("get_pending_custom_transports", CommandType.StoredProcedure,
                static reader => new CustomTransport
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    DriverId = reader.GetInt32OrDefault(2),
                    ScheduledDate = reader.GetDateTimeOrDefault(3)
                }, parameters);

            return rows;
        }

        public async Task<List<CustomTransport>> GetCompletedCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = vehicleId,
                ["p_driver_id"] = driverId
            };

            List<CustomTransport> rows = await _dataAccess.ExecuteReader("get_completed_custom_transports", CommandType.StoredProcedure,
                static reader => new CustomTransport
                {
                    Id = reader.GetInt32OrDefault(0),
                    VehicleId = reader.GetInt32OrDefault(1),
                    DriverId = reader.GetInt32OrDefault(2),
                    CompletedDate = reader.GetDateTimeOrDefault(3)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertCustomTransport(int companyId, CustomTransport customTransport)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_vehicle_id"] = customTransport.VehicleId,
                ["p_driver_id"] = customTransport.DriverId,
                ["p_amount"] = customTransport.Amount,
                ["p_description"] = customTransport.Description,
                ["p_scheduled_date"] = customTransport.ScheduledDate
            };

            object? result = await _dataAccess.ExecuteScalar("insert_custom_transport", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateCustomTransport(int companyId, int id, CustomTransport customTransport)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_vehicle_id"] = customTransport.VehicleId,
                ["p_driver_id"] = customTransport.DriverId,
                ["p_amount"] = customTransport.Amount,
                ["p_description"] = customTransport.Description,
                ["p_scheduled_date"] = customTransport.ScheduledDate
            };

            return await _dataAccess.ExecuteNonQuery("update_custom_transport", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteCustomTransport(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await _dataAccess.ExecuteNonQuery("delete_custom_transport", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> CompleteCustomTransport(int companyId, int id, CustomTransport customTransport)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_completed_date"] = customTransport.CompletedDate
            };

            return await _dataAccess.ExecuteNonQuery("complete_custom_transport", CommandType.StoredProcedure, parameters);
        }
    }
}