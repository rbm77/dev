using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISalaryService
    {

        Task<int> InsertSalary(int companyId, int employeeId, Salary salary);

        Task<List<Salary>> GetSalaries(int companyId, int employeeId);

        Task<bool> DeleteSalary(int companyId, int employeeId, int salaryId);
    }
}
