using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISalaryExpenseRepository
    {
        Task<SalaryExpense?> GetSalaryExpense(int companyId, long id);

        Task<List<SalaryExpense>> GetSalaryExpenses(
            int companyId,
            DateTime? date = null,
            int? employeeId = null
        );

        Task<long> InsertSalaryExpense(int companyId, SalaryExpense expense);

        Task<int> UpdateSalaryExpense(int companyId, long id, SalaryExpense expense);
    }
}
