using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class ExtraSalaryService(IExtraSalaryRepository extraSalaryRepository) : IExtraSalaryService
    {

        private readonly IExtraSalaryRepository _extraSalaryRepository = extraSalaryRepository;

        public async Task<int> InsertExtraSalary(int companyId, int employeeId, ExtraSalary extraSalary)
        {
            return await _extraSalaryRepository.InsertExtraSalary(companyId, employeeId, extraSalary);
        }

        public async Task<List<ExtraSalary>> GetExtraSalaries(int companyId, int employeeId)
        {
            return await _extraSalaryRepository.GetExtraSalaries(companyId, employeeId);
        }

        public async Task<bool> DeleteExtraSalary(int companyId, int employeeId, int extraSalaryId)
        {
            int affected = await _extraSalaryRepository.DeleteExtraSalary(companyId, employeeId, extraSalaryId);
            return affected > 0;
        }
    }
}