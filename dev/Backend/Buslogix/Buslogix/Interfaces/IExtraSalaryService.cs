using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IExtraSalaryService
    {

        Task<int> InsertExtraSalary(int companyId, int employeeId, ExtraSalary extraSalary);

        Task<List<ExtraSalary>> GetExtraSalaries(int companyId, int employeeId);

        Task<bool> DeleteExtraSalary(int companyId, int employeeId, int extraSalaryId);
    }
}
