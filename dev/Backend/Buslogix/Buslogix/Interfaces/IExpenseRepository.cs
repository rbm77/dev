using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IExpenseRepository
    {
        Task<Expense?> GetExpense(int companyId, long id);

        Task<List<Expense>> GetExpenses(
            int companyId,
            DateTime? date = null
        );

        Task<long> InsertExpense(int companyId, Expense expense);

        Task<int> UpdateExpense(int companyId, long id, Expense expense);

        Task<int> DeleteExpense(int companyId, long id);
    }
}