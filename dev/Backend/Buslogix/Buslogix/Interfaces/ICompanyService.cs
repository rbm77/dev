using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ICompanyService
    {
        Task<Company?> GetCompany(int id);

        Task<bool> UpdateCompany(int id, Company company);
    }
}
