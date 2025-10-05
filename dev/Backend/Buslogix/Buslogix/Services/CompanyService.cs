using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
    {

        public async Task<Company?> GetCompany(int id)
        {
            return await companyRepository.GetCompany(id);
        }

        public async Task<bool> UpdateCompany(int id, Company company)
        {
            return (await companyRepository.UpdateCompany(id, company)) > 0;
        }
    }
}
