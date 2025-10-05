using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class MaintenanceExpenseService(IMaintenanceExpenseRepository maintenanceExpenseRepository) : IMaintenanceExpenseService
    {

        public async Task<MaintenanceExpense?> GetMaintenanceExpense(int companyId, long id)
        {
            return await maintenanceExpenseRepository.GetMaintenanceExpense(companyId, id);
        }

        public async Task<List<MaintenanceExpense>> GetMaintenanceExpenses(
            int companyId,
            DateTime? date = null,
            int? maintenanceId = null
        )
        {
            return await maintenanceExpenseRepository.GetMaintenanceExpenses(companyId, date, maintenanceId);
        }

        public async Task<long> InsertMaintenanceExpense(int companyId, MaintenanceExpense expense)
        {
            return await maintenanceExpenseRepository.InsertMaintenanceExpense(companyId, expense);
        }

        public async Task<bool> UpdateMaintenanceExpense(int companyId, long id, MaintenanceExpense expense)
        {
            int affected = await maintenanceExpenseRepository.UpdateMaintenanceExpense(companyId, id, expense);
            return affected > 0;
        }
    }
}