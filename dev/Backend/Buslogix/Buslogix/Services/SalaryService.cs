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

        public async Task<List<Salary>> GetSalaries(int companyId, int employeeId)
        {
            return await salaryRepository.GetSalaries(companyId, employeeId);
        }

        public async Task<bool> DeleteSalary(int companyId, int employeeId, int salaryId)
        {
            int affected = await salaryRepository.DeleteSalary(companyId, employeeId, salaryId);
            return affected > 0;
        }
    }
}
