using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetCompanies();
    }
}
