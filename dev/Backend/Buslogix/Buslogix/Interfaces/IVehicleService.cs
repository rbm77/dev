using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IVehicleService
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

        Task<bool> UpdateVehicle(int companyId, int id, Vehicle vehicle);

        Task<bool> DeleteVehicle(int companyId, int id);
    }
}
