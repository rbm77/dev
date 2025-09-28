using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IFuelExpenseRepository
    {
        Task<FuelExpense?> GetFuelExpense(int companyId, long id);

        Task<List<FuelExpense>> GetFuelExpenses(
            int companyId,
            DateTime? date = null,
            int? vehicleId = null,
            int? driverId = null
        );

        Task<long> InsertFuelExpense(int companyId, FuelExpense expense);

        Task<int> UpdateFuelExpense(int companyId, long id, FuelExpense expense);
    }
}