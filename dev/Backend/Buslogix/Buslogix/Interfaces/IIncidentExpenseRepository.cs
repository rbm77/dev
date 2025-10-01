using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IIncidentExpenseRepository
    {
        Task<IncidentExpense?> GetIncidentExpense(int companyId, long id);

        Task<List<IncidentExpense>> GetIncidentExpenses(
            int companyId,
            DateTime? date = null,
            int? incidentId = null
        );

        Task<long> InsertIncidentExpense(int companyId, IncidentExpense expense);

        Task<int> UpdateIncidentExpense(int companyId, long id, IncidentExpense expense);
    }
}