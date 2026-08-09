using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
    {

        public async Task<Employee?> GetEmployee(int companyId, int id)
        {
            return await employeeRepository.GetEmployee(companyId, id);
        }

        public async Task<PagedResult<Employee>> GetEmployees(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null,
            int page = 1,
            int pageSize = 20
        )
        {
            return await employeeRepository.GetEmployees(companyId, isActive, identityDocument, name, lastName, page, pageSize);
        }

        public async Task<int> InsertEmployee(int companyId, Employee employee)
        {
            return await employeeRepository.InsertEmployee(companyId, employee);
        }

        public async Task<bool> UpdateEmployee(int companyId, int id, Employee employee)
        {
            int affected = await employeeRepository.UpdateEmployee(companyId, id, employee);
            return affected > 0;
        }

        public async Task<bool> DeleteEmployee(int companyId, int id)
        {
            int affected = await employeeRepository.DeleteEmployee(companyId, id);
            return affected > 0;
        }
    }
}
