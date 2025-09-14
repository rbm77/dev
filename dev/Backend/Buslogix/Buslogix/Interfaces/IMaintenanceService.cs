using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Interfaces
{
    public interface IMaintenanceService
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

        Task<bool> UpdateMaintenance(int companyId, int id, Maintenance maintenance);

        Task<bool> DeleteMaintenance(int companyId, int id);
    }
}
