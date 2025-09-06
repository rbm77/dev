using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class VehicleService(IVehicleRepository vehicleRepository) : IVehicleService
    {

        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

        public async Task<Vehicle?> GetVehicle(int companyId, int id)
        {
            return await _vehicleRepository.GetVehicle(companyId, id);
        }

        public async Task<List<Vehicle>> GetVehicles(
            int companyId,
            bool? isActive = null,
            string? licensePlate = null,
            string? make = null,
            string? model = null
        )
        {
            return await _vehicleRepository.GetVehicles(companyId, isActive, licensePlate, make, model);
        }

        public async Task<int> InsertVehicle(int companyId, Vehicle vehicle)
        {
            return await _vehicleRepository.InsertVehicle(companyId, vehicle);
        }

        public async Task<bool> UpdateVehicle(int companyId, int id, Vehicle vehicle)
        {
            int affected = await _vehicleRepository.UpdateVehicle(companyId, id, vehicle);
            return affected > 0;
        }

        public async Task<bool> DeleteVehicle(int companyId, int id)
        {
            int affected = await _vehicleRepository.DeleteVehicle(companyId, id);
            return affected > 0;
        }
    }
}
