using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IMaintenanceExpenseService
    {
        Task<MaintenanceExpense?> GetMaintenanceExpense(int companyId, long id);

        Task<List<MaintenanceExpense>> GetMaintenanceExpenses(
            int companyId,
            DateTime? date = null,
            int? maintenanceId = null
        );

        Task<long> InsertMaintenanceExpense(int companyId, MaintenanceExpense expense);

        Task<bool> UpdateMaintenanceExpense(int companyId, long id, MaintenanceExpense expense);
    }
}