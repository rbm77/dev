using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISalaryExpenseRepository
    {
        Task<SalaryExpense?> GetSalaryExpense(int companyId, long id);

        Task<PagedResult<SalaryExpense>> GetSalaryExpenses(
            int companyId,
            DateTime? date,
            int? employeeId,
            int page,
            int pageSize
        );

        Task<long> InsertSalaryExpense(int companyId, SalaryExpense expense);

        Task<int> UpdateSalaryExpense(int companyId, long id, SalaryExpense expense);
    }
}
