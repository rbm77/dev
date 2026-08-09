using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class SalaryService(ISalaryRepository salaryRepository) : ISalaryService
    {

        public async Task<int> InsertSalary(int companyId, int employeeId, Salary salary)
        {
            return await salaryRepository.InsertSalary(companyId, employeeId, salary);
        }

        public async Task<PagedResult<Salary>> GetSalaries(int companyId, int employeeId, int page, int pageSize)
        {
            return await salaryRepository.GetSalaries(companyId, employeeId, page, pageSize);
        }

        public async Task<bool> DeleteSalary(int companyId, int employeeId, int salaryId)
        {
            int affected = await salaryRepository.DeleteSalary(companyId, employeeId, salaryId);
            return affected > 0;
        }
    }
}
