using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPeriodicExemptionRepository
    {
        Task<PeriodicExemption?> GetPeriodicExemption(int companyId, int id);

        Task<List<PeriodicExemption>> GetPeriodicExemptions(
            int companyId,
            int? studentId = null
        );

        Task<int> InsertPeriodicExemption(int companyId, PeriodicExemption exemption);

        Task<int> UpdatePeriodicExemption(int companyId, int id, PeriodicExemption exemption);

        Task<int> DeletePeriodicExemption(int companyId, int id);
    }
}