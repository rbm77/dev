using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployee(int companyId, int id);

        Task<PagedResult<Employee>> GetEmployees(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null,
            int page = 1,
            int pageSize = 20
        );

        Task<int> InsertEmployee(int companyId, Employee employee);

        Task<int> UpdateEmployee(int companyId, int id, Employee employee);

        Task<int> DeleteEmployee(int companyId, int id);
    }
}