using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IIncidentExpenseService
    {
        Task<IncidentExpense?> GetIncidentExpense(int companyId, long id);

        Task<List<IncidentExpense>> GetIncidentExpenses(
            int companyId,
            DateTime? date = null,
            int? incidentId = null
        );

        Task<long> InsertIncidentExpense(int companyId, IncidentExpense expense);

        Task<bool> UpdateIncidentExpense(int companyId, long id, IncidentExpense expense);
    }
}