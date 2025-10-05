using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class SalaryRepository(IDataAccess dataAccess) : ISalaryRepository
    {

        public async Task<int> InsertSalary(int companyId, int employeeId, Salary salary)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_employee_id"] = employeeId,
                ["p_amount"] = salary.Amount
            };

            object? result = await dataAccess.ExecuteScalar("insert_salary", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<List<Salary>> GetSalaries(int companyId, int employeeId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_employee_id"] = employeeId
            };

            List<Salary> rows = await dataAccess.ExecuteReader("get_salaries", CommandType.StoredProcedure,
                static reader => new Salary
                {
                    Id = reader.GetInt32OrDefault(0),
                    Amount = reader.GetDecimalOrDefault(1),
                    StartDate = reader.GetDateTimeOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<int> DeleteSalary(int companyId, int employeeId, int salaryId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_employee_id"] = employeeId,
                ["p_id"] = salaryId
            };

            return await dataAccess.ExecuteNonQuery("delete_salary", CommandType.StoredProcedure, parameters);
        }
    }
}