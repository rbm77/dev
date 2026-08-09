using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IVehicleRepository
    {

        Task<Vehicle?> GetVehicle(int companyId, int id);

        Task<PagedResult<Vehicle>> GetVehicles(
            int companyId,
            bool? isActive,
            string? licensePlate,
            string? make,
            string? model,
            int page,
            int pageSize
        );

        Task<int> InsertVehicle(int companyId, Vehicle vehicle);

        Task<int> UpdateVehicle(int companyId, int id, Vehicle vehicle);

        Task<int> DeleteVehicle(int companyId, int id);

    }
}