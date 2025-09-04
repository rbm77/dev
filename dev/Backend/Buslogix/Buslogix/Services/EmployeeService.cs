using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository = employeeRepository;

        public async Task<Employee?> GetEmployee(int companyId, int id)
        {
            return await _employeeRepository.GetEmployee(companyId, id);
        }

        public async Task<List<Employee>> GetEmployees(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null
        )
        {
            return await _employeeRepository.GetEmployees(companyId, isActive, identityDocument, name, lastName);
        }

        public async Task<int> InsertEmployee(int companyId, Employee employee)
        {
            return await _employeeRepository.InsertEmployee(companyId, employee);
        }

        public async Task<bool> UpdateEmployee(int companyId, int id, Employee employee)
        {
            int affected = await _employeeRepository.UpdateEmployee(companyId, id, employee);
            return affected > 0;
        }

        public async Task<bool> DeleteEmployee(int companyId, int id)
        {
            int affected = await _employeeRepository.DeleteEmployee(companyId, id);
            return affected > 0;
        }
    }
}
