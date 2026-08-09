using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class VacationService(IVacationRepository vacationRepository) : IVacationService
    {

        public async Task<Vacation?> GetVacation(int companyId, int id)
        {
            return await vacationRepository.GetVacation(companyId, id);
        }

        public async Task<PagedResult<Vacation>> GetAllVacation(int companyId, int page, int pageSize)
        {
            return await vacationRepository.GetAllVacation(companyId, page, pageSize);
        }

        public async Task<int> InsertVacation(int companyId, Vacation vacation)
        {
            return await vacationRepository.InsertVacation(companyId, vacation);
        }

        public async Task<bool> UpdateVacation(int companyId, int id, Vacation vacation)
        {
            int affected = await vacationRepository.UpdateVacation(companyId, id, vacation);
            return affected > 0;
        }

        public async Task<bool> DeleteVacation(int companyId, int id)
        {
            int affected = await vacationRepository.DeleteVacation(companyId, id);
            return affected > 0;
        }
    }
}