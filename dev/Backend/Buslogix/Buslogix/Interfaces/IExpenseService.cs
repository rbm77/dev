using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IExpenseService
    {
        Task<Expense?> GetExpense(int companyId, long id);

        Task<List<Expense>> GetExpenses(
            int companyId,
            DateTime? date = null
        );

        Task<long> InsertExpense(int companyId, Expense expense);

        Task<bool> UpdateExpense(int companyId, long id, Expense expense);

        Task<bool> DeleteExpense(int companyId, long id);
    }
}