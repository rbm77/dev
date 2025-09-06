using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class ExtraSalaryRepository(IDataAccess dataAccess) : IExtraSalaryRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<int> InsertExtraSalary(int companyId, int employeeId, ExtraSalary extraSalary)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_employee_id"] = employeeId,
                ["p_amount"] = extraSalary.Amount,
                ["p_description"] = extraSalary.Description
            };

            object? result = await _dataAccess.ExecuteScalar("insert_extra_salary", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<List<ExtraSalary>> GetExtraSalaries(int companyId, int employeeId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_employee_id"] = employeeId
            };

            List<ExtraSalary> rows = await _dataAccess.ExecuteReader("get_extra_salaries", CommandType.StoredProcedure,
                static reader => new ExtraSalary
                {
                    Id = reader.GetInt32OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    Description = reader.GetStringOrDefault(3)
                }, parameters);

            return rows;
        }

        public async Task<int> DeleteExtraSalary(int companyId, int employeeId, int extraSalaryId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_employee_id"] = employeeId,
                ["p_id"] = extraSalaryId
            };

            return await _dataAccess.ExecuteNonQuery("delete_extra_salary", CommandType.StoredProcedure, parameters);
        }
    }
}