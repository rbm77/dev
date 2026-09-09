
namespace Buslogix.Features.Companies
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompany(int id);

        Task<int> UpdateCompany(int id, Company company);
    }
}
