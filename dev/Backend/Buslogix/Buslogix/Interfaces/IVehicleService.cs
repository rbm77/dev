using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IVehicleService
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

        Task<bool> UpdateVehicle(int companyId, int id, Vehicle vehicle);

        Task<bool> DeleteVehicle(int companyId, int id);
    }
}
