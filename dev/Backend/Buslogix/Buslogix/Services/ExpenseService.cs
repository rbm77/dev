using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class ExpenseService(IExpenseRepository expenseRepository) : IExpenseService
    {

        private readonly IExpenseRepository _expenseRepository = expenseRepository;

        public async Task<Expense?> GetExpense(int companyId, long id)
        {
            return await _expenseRepository.GetExpense(companyId, id);
        }

        public async Task<List<Expense>> GetExpenses(
            int companyId,
            DateTime? date = null
        )
        {
            return await _expenseRepository.GetExpenses(companyId, date);
        }

        public async Task<long> InsertExpense(int companyId, Expense expense)
        {
            return await _expenseRepository.InsertExpense(companyId, expense);
        }

        public async Task<bool> UpdateExpense(int companyId, long id, Expense expense)
        {
            int affected = await _expenseRepository.UpdateExpense(companyId, id, expense);
            return affected > 0;
        }

        public async Task<bool> DeleteExpense(int companyId, long id)
        {
            int affected = await _expenseRepository.DeleteExpense(companyId, id);
            return affected > 0;
        }
    }
}
