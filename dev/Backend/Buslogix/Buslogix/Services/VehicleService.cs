using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class VehicleService(IVehicleRepository vehicleRepository) : IVehicleService
    {

        public async Task<Vehicle?> GetVehicle(int companyId, int id)
        {
            return await vehicleRepository.GetVehicle(companyId, id);
        }

        public async Task<PagedResult<Vehicle>> GetVehicles(
            int companyId,
            bool? isActive,
            string? licensePlate,
            string? make,
            string? model,
            int page,
            int pageSize
        )
        {
            return await vehicleRepository.GetVehicles(companyId, isActive, licensePlate, make, model, page, pageSize);
        }

        public async Task<int> InsertVehicle(int companyId, Vehicle vehicle)
        {
            return await vehicleRepository.InsertVehicle(companyId, vehicle);
        }

        public async Task<bool> UpdateVehicle(int companyId, int id, Vehicle vehicle)
        {
            int affected = await vehicleRepository.UpdateVehicle(companyId, id, vehicle);
            return affected > 0;
        }

        public async Task<bool> DeleteVehicle(int companyId, int id)
        {
            int affected = await vehicleRepository.DeleteVehicle(companyId, id);
            return affected > 0;
        }
    }
}
