using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IEmailAccountRepository
    {

        /// <summary>Pass companyId: null and isActive: true for a cross-tenant, active-only fetch (the polling job).</summary>
        Task<PagedResult<EmailAccount>> GetEmailAccounts(int? companyId, bool? isActive, int page, int pageSize);

        Task<int> InsertEmailAccount(int companyId, EmailAccount emailAccount);

        Task<int> UpdateEmailAccount(int companyId, int id, EmailAccount emailAccount);

        Task<int> DeleteEmailAccount(int companyId, int id);

        Task UpdateLastChecked(int companyId, int id);
    }
}
