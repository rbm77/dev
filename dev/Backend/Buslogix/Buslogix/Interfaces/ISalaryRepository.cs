using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISalaryRepository
    {
        Task<int> InsertSalary(int companyId, int employeeId, Salary salary);

        Task<List<Salary>> GetSalaries(int companyId, int employeeId);

        Task<int> DeleteSalary(int companyId, int employeeId, int salaryId);
    }
}
