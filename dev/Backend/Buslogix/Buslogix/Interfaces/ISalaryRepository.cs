using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISalaryRepository
    {
        Task<int> InsertSalary(int companyId, int employeeId, Salary salary);

        Task<PagedResult<Salary>> GetSalaries(int companyId, int employeeId, int page, int pageSize);

        Task<int> DeleteSalary(int companyId, int employeeId, int salaryId);
    }
}
