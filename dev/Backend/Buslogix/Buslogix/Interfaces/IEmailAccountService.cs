using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IEmailAccountService
    {

        Task<PagedResult<EmailAccount>> GetEmailAccounts(int companyId, bool? isActive, int page, int pageSize);

        Task<int> InsertEmailAccount(int companyId, EmailAccount emailAccount);

        Task<bool> UpdateEmailAccount(int companyId, int id, EmailAccount emailAccount);

        Task<bool> DeleteEmailAccount(int companyId, int id);
    }
}
