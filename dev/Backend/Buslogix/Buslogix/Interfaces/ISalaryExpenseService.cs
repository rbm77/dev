using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISalaryExpenseService
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

        Task<bool> UpdateSalaryExpense(int companyId, long id, SalaryExpense expense);
    }
}