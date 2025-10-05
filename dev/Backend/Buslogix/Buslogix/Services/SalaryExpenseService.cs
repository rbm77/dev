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

        public async Task<List<SalaryExpense>> GetSalaryExpenses(
            int companyId,
            DateTime? date = null,
            int? employeeId = null
        )
        {
            return await salaryExpenseRepository.GetSalaryExpenses(companyId, date, employeeId);
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