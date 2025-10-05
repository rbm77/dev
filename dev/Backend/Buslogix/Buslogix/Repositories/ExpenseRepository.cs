using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using System.Data;

namespace Buslogix.Repositories
{
    public class ExpenseRepository(IDataAccess dataAccess) : IExpenseRepository
    {

        public async Task<Expense?> GetExpense(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Expense> rows = await dataAccess.ExecuteReader("get_expense", CommandType.StoredProcedure,
                static reader => new Expense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    Description = reader.GetStringOrDefault(3)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Expense>> GetExpenses(
            int companyId,
            DateTime? date = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = date
            };

            List<Expense> rows = await dataAccess.ExecuteReader("get_expenses", CommandType.StoredProcedure,
                static reader => new Expense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<long> InsertExpense(int companyId, Expense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description
            };

            object? result = await dataAccess.ExecuteScalar("insert_expense", CommandType.StoredProcedure, parameters);
            return result != null ? Convert.ToInt64(result) : 0L;
        }

        public async Task<int> UpdateExpense(int companyId, long id, Expense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description
            };

            return await dataAccess.ExecuteNonQuery("update_expense", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteExpense(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_expense", CommandType.StoredProcedure, parameters);
        }
    }
}