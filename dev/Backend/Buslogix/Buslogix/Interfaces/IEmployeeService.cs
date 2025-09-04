using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployee(int companyId, int id);

        Task<List<Employee>> GetEmployees(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null
        );

        Task<int> InsertEmployee(int companyId, Employee employee);

        Task<bool> UpdateEmployee(int companyId, int id, Employee employee);

        Task<bool> DeleteEmployee(int companyId, int id);
    }
}