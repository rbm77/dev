using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
    {

        private readonly ICompanyRepository _companyRepository = companyRepository;

        public async Task<Company?> GetCompany()
        {
            return await _companyRepository.GetCompany();
        }

        public async Task<bool> UpdateCompany(Company company)
        {
            return (await _companyRepository.UpdateCompany(company)) > 0;
        }
    }
}
