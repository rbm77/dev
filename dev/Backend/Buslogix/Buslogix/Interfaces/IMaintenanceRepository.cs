using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Interfaces
{
    public interface IMaintenanceRepository
    {
        Task<Maintenance?> GetMaintenance(int companyId, int id);

        Task<List<Maintenance>> GetPendingMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        );

        Task<List<Maintenance>> GetCompletedMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        );

        Task<int> InsertMaintenance(int companyId, Maintenance maintenance);

        Task<int> UpdateMaintenance(int companyId, int id, Maintenance maintenance);

        Task<int> DeleteMaintenance(int companyId, int id);
    }
}