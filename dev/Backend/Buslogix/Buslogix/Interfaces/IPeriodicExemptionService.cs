using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPeriodicExemptionService
    {
        Task<PeriodicExemption?> GetPeriodicExemption(int companyId, int id);

        Task<List<PeriodicExemption>> GetPeriodicExemptions(
            int companyId,
            int? studentId = null
        );

        Task<int> InsertPeriodicExemption(int companyId, PeriodicExemption exemption);

        Task<bool> UpdatePeriodicExemption(int companyId, int id, PeriodicExemption exemption);

        Task<bool> DeletePeriodicExemption(int companyId, int id);
    }
}