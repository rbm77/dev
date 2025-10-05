using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using System.Data;

namespace Buslogix.Repositories
{
    public class SalaryExpenseRepository(IDataAccess dataAccess) : ISalaryExpenseRepository
    {

        public async Task<SalaryExpense?> GetSalaryExpense(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<SalaryExpense> rows = await dataAccess.ExecuteReader("get_salary_expense", CommandType.StoredProcedure,
                static reader => new SalaryExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    Description = reader.GetStringOrDefault(3),
                    EmployeeId = reader.GetInt32OrDefault(4)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<SalaryExpense>> GetSalaryExpenses(
            int companyId,
            DateTime? date = null,
            int? employeeId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = date,
                ["p_employee_id"] = employeeId
            };

            List<SalaryExpense> rows = await dataAccess.ExecuteReader("get_salary_expenses", CommandType.StoredProcedure,
                static reader => new SalaryExpense
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<long> InsertSalaryExpense(int companyId, SalaryExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_employee_id"] = expense.EmployeeId
            };

            object? result = await dataAccess.ExecuteScalar("insert_salary_expense", CommandType.StoredProcedure, parameters);
            return result != null ? Convert.ToInt64(result) : 0L;
        }

        public async Task<int> UpdateSalaryExpense(int companyId, long id, SalaryExpense expense)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_date"] = expense.Date,
                ["p_amount"] = expense.Amount,
                ["p_description"] = expense.Description,
                ["p_employee_id"] = expense.EmployeeId
            };

            return await dataAccess.ExecuteNonQuery("update_salary_expense", CommandType.StoredProcedure, parameters);
        }
    }
}