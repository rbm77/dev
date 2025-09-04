using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IEmployeeRepository
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

        Task<int> UpdateEmployee(int companyId, int id, Employee employee);

        Task<int> DeleteEmployee(int companyId, int id);
    }
}