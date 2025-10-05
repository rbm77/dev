using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IVacationService
    {
        Task<Vacation?> GetVacation(int companyId, int id);

        Task<List<Vacation>> GetAllVacation(int companyId);

        Task<int> InsertVacation(int companyId, Vacation vacation);

        Task<bool> UpdateVacation(int companyId, int id, Vacation vacation);

        Task<bool> DeleteVacation(int companyId, int id);
    }
}