using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using System.Data;

namespace Buslogix.Repositories
{
    public class FuelExpenseRepository(IDataAccess dataAccess) : IFuelExpenseRepository
    {

        public async Task<FuelExpense?> GetFuelExpense(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<FuelExpense> rows = await dataAccess.ExecuteReader("get_fuel_expense", CommandType.StoredProcedure,
                static reader => new FuelExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    Description = reader.GetStringOrDefault(3),
                    VehicleId = reader.GetInt32OrDefault(4),
                    DriverId = reader.GetInt32OrDefault(5),
                    Liters = reader.GetInt32OrDefault(6)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<FuelExpense>> GetFuelExpenses(
            int companyId,
            DateTime? date = null,
            int? vehicleId = null,
            int? driverId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = date,
                ["p_vehicle_id"] = vehicleId,
                ["p_driver_id"] = driverId
            };

            List<FuelExpense> rows = await dataAccess.ExecuteReader("get_fuel_expenses", CommandType.StoredProcedure,
                static reader => new FuelExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<long> InsertFuelExpense(int companyId, FuelExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_vehicle_id"] = expense.VehicleId,
                ["p_driver_id"] = expense.DriverId,
                ["p_liters"] = expense.Liters
            };

            object? result = await dataAccess.ExecuteScalar("insert_fuel_expense", CommandType.StoredProcedure, parameters);
            return result != null ? Convert.ToInt64(result) : 0L;
        }

        public async Task<int> UpdateFuelExpense(int companyId, long id, FuelExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_vehicle_id"] = expense.VehicleId,
                ["p_driver_id"] = expense.DriverId,
                ["p_liters"] = expense.Liters
            };

            return await dataAccess.ExecuteNonQuery("update_fuel_expense", CommandType.StoredProcedure, parameters);
        }
    }
}