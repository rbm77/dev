
namespace Buslogix.Features.EmailAccounts
{
    public interface IEmailAccountRepository
    {

        Task<PagedResult<EmailAccount>> GetEmailAccounts(int? companyId, bool? isActive, int page, int pageSize);

        Task<int> InsertEmailAccount(int companyId, EmailAccount emailAccount);

        Task<int> UpdateEmailAccount(int companyId, int id, EmailAccount emailAccount);

        Task<int> DeleteEmailAccount(int companyId, int id);

        Task UpdateLastChecked(int companyId, int id);
    }
}
