using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class FuelExpenseService(IFuelExpenseRepository fuelExpenseRepository) : IFuelExpenseService
    {

        private readonly IFuelExpenseRepository _fuelExpenseRepository = fuelExpenseRepository;

        public async Task<FuelExpense?> GetFuelExpense(int companyId, long id)
        {
            return await _fuelExpenseRepository.GetFuelExpense(companyId, id);
        }

        public async Task<List<FuelExpense>> GetFuelExpenses(
            int companyId,
            DateTime? date = null,
            int? vehicleId = null,
            int? driverId = null
        )
        {
            return await _fuelExpenseRepository.GetFuelExpenses(companyId, date, vehicleId, driverId);
        }

        public async Task<long> InsertFuelExpense(int companyId, FuelExpense expense)
        {
            return await _fuelExpenseRepository.InsertFuelExpense(companyId, expense);
        }

        public async Task<bool> UpdateFuelExpense(int companyId, long id, FuelExpense expense)
        {
            int affected = await _fuelExpenseRepository.UpdateFuelExpense(companyId, id, expense);
            return affected > 0;
        }
    }
}
