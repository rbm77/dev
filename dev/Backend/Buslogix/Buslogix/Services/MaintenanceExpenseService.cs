using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class MaintenanceExpenseService(IMaintenanceExpenseRepository maintenanceExpenseRepository) : IMaintenanceExpenseService
    {

        private readonly IMaintenanceExpenseRepository _maintenanceExpenseRepository = maintenanceExpenseRepository;

        public async Task<MaintenanceExpense?> GetMaintenanceExpense(int companyId, long id)
        {
            return await _maintenanceExpenseRepository.GetMaintenanceExpense(companyId, id);
        }

        public async Task<List<MaintenanceExpense>> GetMaintenanceExpenses(
            int companyId,
            DateTime? date = null,
            int? maintenanceId = null
        )
        {
            return await _maintenanceExpenseRepository.GetMaintenanceExpenses(companyId, date, maintenanceId);
        }

        public async Task<long> InsertMaintenanceExpense(int companyId, MaintenanceExpense expense)
        {
            return await _maintenanceExpenseRepository.InsertMaintenanceExpense(companyId, expense);
        }

        public async Task<bool> UpdateMaintenanceExpense(int companyId, long id, MaintenanceExpense expense)
        {
            int affected = await _maintenanceExpenseRepository.UpdateMaintenanceExpense(companyId, id, expense);
            return affected > 0;
        }
    }
}