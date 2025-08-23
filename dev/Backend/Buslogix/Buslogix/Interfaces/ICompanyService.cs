using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ICompanyService
    {
        Task<Company?> GetCompany();

        Task<bool> UpdateCompany(Company company);
    }
}
