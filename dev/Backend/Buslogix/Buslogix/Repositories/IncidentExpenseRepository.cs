using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using System.Data;

namespace Buslogix.Repositories
{
    public class IncidentExpenseRepository(IDataAccess dataAccess) : IIncidentExpenseRepository
    {

        public async Task<IncidentExpense?> GetIncidentExpense(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<IncidentExpense> rows = await dataAccess.ExecuteReader("get_incident_expense", CommandType.StoredProcedure,
                static reader => new IncidentExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    Description = reader.GetStringOrDefault(3),
                    IncidentId = reader.GetInt32OrDefault(4)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<IncidentExpense>> GetIncidentExpenses(
            int companyId,
            DateTime? date = null,
            int? incidentId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = date,
                ["p_incident_id"] = incidentId
            };

            List<IncidentExpense> rows = await dataAccess.ExecuteReader("get_incident_expenses", CommandType.StoredProcedure,
                static reader => new IncidentExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<long> InsertIncidentExpense(int companyId, IncidentExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_incident_id"] = expense.IncidentId
            };

            object? result = await dataAccess.ExecuteScalar("insert_incident_expense", CommandType.StoredProcedure, parameters);
            return result != null ? Convert.ToInt64(result) : 0L;
        }

        public async Task<int> UpdateIncidentExpense(int companyId, long id, IncidentExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_incident_id"] = expense.IncidentId
            };

            return await dataAccess.ExecuteNonQuery("update_incident_expense", CommandType.StoredProcedure, parameters);
        }
    }
}