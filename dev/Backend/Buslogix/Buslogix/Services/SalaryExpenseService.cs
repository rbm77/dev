using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class SalaryExpenseService(ISalaryExpenseRepository salaryExpenseRepository) : ISalaryExpenseService
    {

        public async Task<SalaryExpense?> GetSalaryExpense(int companyId, long id)
        {
            return await salaryExpenseRepository.GetSalaryExpense(companyId, id);
        }

        public async Task<PagedResult<SalaryExpense>> GetSalaryExpenses(
            int companyId,
            DateTime? date,
            int? employeeId,
            int page,
            int pageSize
        )
        {
            return await salaryExpenseRepository.GetSalaryExpenses(companyId, date, employeeId, page, pageSize);
        }

        public async Task<long> InsertSalaryExpense(int companyId, SalaryExpense expense)
        {
            return await salaryExpenseRepository.InsertSalaryExpense(companyId, expense);
        }

        public async Task<bool> UpdateSalaryExpense(int companyId, long id, SalaryExpense expense)
        {
            int affected = await salaryExpenseRepository.UpdateSalaryExpense(companyId, id, expense);
            return affected > 0;
        }
    }
}