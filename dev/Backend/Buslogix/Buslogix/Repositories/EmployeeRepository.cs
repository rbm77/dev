using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class EmployeeRepository(IDataAccess dataAccess) : IEmployeeRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<Employee?> GetEmployee(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Employee> rows = await _dataAccess.ExecuteReader("get_employee", CommandType.StoredProcedure,
                static reader => new Employee
                {
                    Id = reader.GetInt32OrDefault(0),
                    HireDate = reader.GetDateTimeOrDefault(1),
                    IsActive = reader.GetBooleanOrDefault(2),

                    IdentityDocument = reader.GetStringOrDefault(3),
                    Name = reader.GetStringOrDefault(4),
                    LastName = reader.GetStringOrDefault(5),
                    Address = reader.GetStringOrDefault(6),
                    PhoneNumber = reader.GetStringOrDefault(7),
                    Email = reader.GetStringOrDefault(8)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Employee>> GetEmployees(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_is_active"] = isActive,
                ["p_identity_document"] = identityDocument,
                ["p_name"] = name,
                ["p_lastname"] = lastName
            };

            List<Employee> rows = await _dataAccess.ExecuteReader("get_employees", CommandType.StoredProcedure,
                static reader => new Employee
                {
                    Id = reader.GetInt32OrDefault(0),
                    HireDate = reader.GetDateTimeOrDefault(1),
                    IsActive = reader.GetBooleanOrDefault(2),

                    IdentityDocument = reader.GetStringOrDefault(3),
                    Name = reader.GetStringOrDefault(4),
                    LastName = reader.GetStringOrDefault(5),
                    Address = reader.GetStringOrDefault(6),
                    PhoneNumber = reader.GetStringOrDefault(7),
                    Email = reader.GetStringOrDefault(8)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertEmployee(int companyId, Employee employee)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_hire_date"] = employee.HireDate,
                ["p_is_active"] = employee.IsActive,

                ["p_identity_document"] = employee.IdentityDocument,
                ["p_name"] = employee.Name,
                ["p_lastname"] = employee.LastName,
                ["p_address"] = employee.Address,
                ["p_phone_number"] = employee.PhoneNumber,
                ["p_email"] = employee.Email
            };

            object? result = await _dataAccess.ExecuteScalar("insert_employee", CommandType.StoredProcedure, parameters);
            return result != null ? ((int)result) : 0;
        }

        public async Task<int> UpdateEmployee(int companyId, int id, Employee employee)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_hire_date"] = employee.HireDate,
                ["p_is_active"] = employee.IsActive,

                ["p_identity_document"] = employee.IdentityDocument,
                ["p_name"] = employee.Name,
                ["p_lastname"] = employee.LastName,
                ["p_address"] = employee.Address,
                ["p_phone_number"] = employee.PhoneNumber,
                ["p_email"] = employee.Email
            };

            return await _dataAccess.ExecuteNonQuery("update_employee", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteEmployee(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await _dataAccess.ExecuteNonQuery("delete_employee", CommandType.StoredProcedure, parameters);
        }
    }
}