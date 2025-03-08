using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository = companyRepository;

        public async Task<List<Company>> GetCompanies()
        {
            return await _companyRepository.GetCompanies();
        }
    }
}
