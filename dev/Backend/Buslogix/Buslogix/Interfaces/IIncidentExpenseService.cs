using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IIncidentExpenseService
    {
        Task<IncidentExpense?> GetIncidentExpense(int companyId, long id);

        Task<PagedResult<IncidentExpense>> GetIncidentExpenses(
            int companyId,
            DateTime? date = null,
            int? incidentId = null,
            int page = 1,
            int pageSize = 20
        );

        Task<long> InsertIncidentExpense(int companyId, IncidentExpense expense);

        Task<bool> UpdateIncidentExpense(int companyId, long id, IncidentExpense expense);
    }
}