using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IVehicleRepository
    {

        Task<Vehicle?> GetVehicle(int companyId, int id);

        Task<List<Vehicle>> GetVehicles(
            int companyId,
            bool? isActive = null,
            string? licensePlate = null,
            string? make = null,
            string? model = null
        );

        Task<int> InsertVehicle(int companyId, Vehicle vehicle);

        Task<int> UpdateVehicle(int companyId, int id, Vehicle vehicle);

        Task<int> DeleteVehicle(int companyId, int id);

    }
}