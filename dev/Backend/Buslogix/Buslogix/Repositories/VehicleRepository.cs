using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class VehicleRepository(IDataAccess dataAccess) : IVehicleRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<Vehicle?> GetVehicle(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Vehicle> rows = await _dataAccess.ExecuteReader("get_vehicle", CommandType.StoredProcedure,
                static reader => new Vehicle
                {
                    Id = reader.GetInt32OrDefault(0),
                    LicensePlate = reader.GetStringOrDefault(1),
                    Make = reader.GetStringOrDefault(2),
                    Model = reader.GetStringOrDefault(3),
                    ManufactureYear = reader.GetInt32OrDefault(4),
                    Capacity = reader.GetInt32OrDefault(5),
                    Mileage = reader.GetInt32OrDefault(6),
                    AcquisitionDate = reader.GetDateTimeOrDefault(7),
                    IsActive = reader.GetBooleanOrDefault(8)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Vehicle>> GetVehicles(
            int companyId,
            bool? isActive = null,
            string? licensePlate = null,
            string? make = null,
            string? model = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_is_active"] = isActive,
                ["p_license_plate"] = licensePlate,
                ["p_make"] = make,
                ["p_model"] = model
            };

            List<Vehicle> rows = await _dataAccess.ExecuteReader("get_vehicles", CommandType.StoredProcedure,
                static reader => new Vehicle
                {
                    Id = reader.GetInt32OrDefault(0),
                    LicensePlate = reader.GetStringOrDefault(1),
                    Make = reader.GetStringOrDefault(2),
                    Model = reader.GetStringOrDefault(3),
                    ManufactureYear = reader.GetInt32OrDefault(4),
                    Capacity = reader.GetInt32OrDefault(5),
                    Mileage = reader.GetInt32OrDefault(6),
                    AcquisitionDate = reader.GetDateTimeOrDefault(7),
                    IsActive = reader.GetBooleanOrDefault(8)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertVehicle(int companyId, Vehicle vehicle)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_license_plate"] = vehicle.LicensePlate,
                ["p_make"] = vehicle.Make,
                ["p_model"] = vehicle.Model,
                ["p_manufacture_year"] = vehicle.ManufactureYear,
                ["p_capacity"] = vehicle.Capacity,
                ["p_mileage"] = vehicle.Mileage,
                ["p_acquisition_date"] = vehicle.AcquisitionDate,
                ["p_is_active"] = vehicle.IsActive
            };

            object? result = await _dataAccess.ExecuteScalar("insert_vehicle", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateVehicle(int companyId, int id, Vehicle vehicle)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_license_plate"] = vehicle.LicensePlate,
                ["p_make"] = vehicle.Make,
                ["p_model"] = vehicle.Model,
                ["p_manufacture_year"] = vehicle.ManufactureYear,
                ["p_capacity"] = vehicle.Capacity,
                ["p_mileage"] = vehicle.Mileage,
                ["p_acquisition_date"] = vehicle.AcquisitionDate,
                ["p_is_active"] = vehicle.IsActive
            };

            return await _dataAccess.ExecuteNonQuery("update_vehicle", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteVehicle(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await _dataAccess.ExecuteNonQuery("delete_vehicle", CommandType.StoredProcedure, parameters);
        }
    }
}