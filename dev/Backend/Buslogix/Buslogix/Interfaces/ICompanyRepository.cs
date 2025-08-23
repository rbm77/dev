using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompany();

        Task<int> UpdateCompany(Company company);
    }
}
