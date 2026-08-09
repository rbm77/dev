using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class FuelExpenseService(IFuelExpenseRepository fuelExpenseRepository) : IFuelExpenseService
    {

        public async Task<FuelExpense?> GetFuelExpense(int companyId, long id)
        {
            return await fuelExpenseRepository.GetFuelExpense(companyId, id);
        }

        public async Task<PagedResult<FuelExpense>> GetFuelExpenses(
            int companyId,
            DateTime? date = null,
            int? vehicleId = null,
            int? driverId = null,
            int page = 1,
            int pageSize = 20
        )
        {
            return await fuelExpenseRepository.GetFuelExpenses(companyId, date, vehicleId, driverId, page, pageSize);
        }

        public async Task<long> InsertFuelExpense(int companyId, FuelExpense expense)
        {
            return await fuelExpenseRepository.InsertFuelExpense(companyId, expense);
        }

        public async Task<bool> UpdateFuelExpense(int companyId, long id, FuelExpense expense)
        {
            int affected = await fuelExpenseRepository.UpdateFuelExpense(companyId, id, expense);
            return affected > 0;
        }
    }
}
