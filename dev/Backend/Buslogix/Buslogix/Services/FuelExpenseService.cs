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

        public async Task<List<FuelExpense>> GetFuelExpenses(
            int companyId,
            DateTime? date = null,
            int? vehicleId = null,
            int? driverId = null
        )
        {
            return await fuelExpenseRepository.GetFuelExpenses(companyId, date, vehicleId, driverId);
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
