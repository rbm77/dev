using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class IncidentExpenseService(IIncidentExpenseRepository incidentExpenseRepository) : IIncidentExpenseService
    {

        public async Task<IncidentExpense?> GetIncidentExpense(int companyId, long id)
        {
            return await incidentExpenseRepository.GetIncidentExpense(companyId, id);
        }

        public async Task<List<IncidentExpense>> GetIncidentExpenses(
            int companyId,
            DateTime? date = null,
            int? incidentId = null
        )
        {
            return await incidentExpenseRepository.GetIncidentExpenses(companyId, date, incidentId);
        }

        public async Task<long> InsertIncidentExpense(int companyId, IncidentExpense expense)
        {
            return await incidentExpenseRepository.InsertIncidentExpense(companyId, expense);
        }

        public async Task<bool> UpdateIncidentExpense(int companyId, long id, IncidentExpense expense)
        {
            int affected = await incidentExpenseRepository.UpdateIncidentExpense(companyId, id, expense);
            return affected > 0;
        }
    }
}