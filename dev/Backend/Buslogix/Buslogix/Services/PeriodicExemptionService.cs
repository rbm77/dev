using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class PeriodicExemptionService(IPeriodicExemptionRepository periodicExemptionRepository) : IPeriodicExemptionService
    {

        public async Task<PeriodicExemption?> GetPeriodicExemption(int companyId, int id)
        {
            return await periodicExemptionRepository.GetPeriodicExemption(companyId, id);
        }

        public async Task<List<PeriodicExemption>> GetPeriodicExemptions(
            int companyId,
            int? studentId = null
        )
        {
            return await periodicExemptionRepository.GetPeriodicExemptions(companyId, studentId);
        }

        public async Task<int> InsertPeriodicExemption(int companyId, PeriodicExemption exemption)
        {
            return await periodicExemptionRepository.InsertPeriodicExemption(companyId, exemption);
        }

        public async Task<bool> UpdatePeriodicExemption(int companyId, int id, PeriodicExemption exemption)
        {
            int affected = await periodicExemptionRepository.UpdatePeriodicExemption(companyId, id, exemption);
            return affected > 0;
        }

        public async Task<bool> DeletePeriodicExemption(int companyId, int id)
        {
            int affected = await periodicExemptionRepository.DeletePeriodicExemption(companyId, id);
            return affected > 0;
        }
    }
}