using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
    {

        private readonly ICompanyRepository _companyRepository = companyRepository;

        public async Task<Company?> GetCompany(int id)
        {
            return await _companyRepository.GetCompany(id);
        }

        public async Task<bool> UpdateCompany(int id, Company company)
        {
            return (await _companyRepository.UpdateCompany(id, company)) > 0;
        }
    }
}
