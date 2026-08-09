using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IVacationRepository
    {
        Task<Vacation?> GetVacation(int companyId, int id);

        Task<PagedResult<Vacation>> GetAllVacation(int companyId, int page, int pageSize);

        Task<int> InsertVacation(int companyId, Vacation vacation);

        Task<int> UpdateVacation(int companyId, int id, Vacation vacation);

        Task<int> DeleteVacation(int companyId, int id);
    }
}