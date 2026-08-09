using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISalaryService
    {

        Task<int> InsertSalary(int companyId, int employeeId, Salary salary);

        Task<PagedResult<Salary>> GetSalaries(int companyId, int employeeId, int page, int pageSize);

        Task<bool> DeleteSalary(int companyId, int employeeId, int salaryId);
    }
}
