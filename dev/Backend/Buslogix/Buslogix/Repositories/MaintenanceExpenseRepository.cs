using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using System.Data;

namespace Buslogix.Repositories
{
    public class MaintenanceExpenseRepository(IDataAccess dataAccess) : IMaintenanceExpenseRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<MaintenanceExpense?> GetMaintenanceExpense(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<MaintenanceExpense> rows = await _dataAccess.ExecuteReader("get_maintenance_expense", CommandType.StoredProcedure,
                static reader => new MaintenanceExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    Description = reader.GetStringOrDefault(3),
                    MaintenanceId = reader.GetInt32OrDefault(4)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<MaintenanceExpense>> GetMaintenanceExpenses(
            int companyId,
            DateTime? date = null,
            int? maintenanceId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = date,
                ["p_maintenance_id"] = maintenanceId
            };

            List<MaintenanceExpense> rows = await _dataAccess.ExecuteReader("get_maintenance_expenses", CommandType.StoredProcedure,
                static reader => new MaintenanceExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<long> InsertMaintenanceExpense(int companyId, MaintenanceExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_maintenance_id"] = expense.MaintenanceId
            };

            object? result = await _dataAccess.ExecuteScalar("insert_maintenance_expense", CommandType.StoredProcedure, parameters);
            return result != null ? Convert.ToInt64(result) : 0L;
        }

        public async Task<int> UpdateMaintenanceExpense(int companyId, long id, MaintenanceExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_maintenance_id"] = expense.MaintenanceId
            };

            return await _dataAccess.ExecuteNonQuery("update_maintenance_expense", CommandType.StoredProcedure, parameters);
        }
    }
}