using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class EmailAccountService(IEmailAccountRepository emailAccountRepository) : IEmailAccountService
    {

        public async Task<PagedResult<EmailAccount>> GetEmailAccounts(int companyId, bool? isActive, int page, int pageSize)
        {
            return await emailAccountRepository.GetEmailAccounts(companyId, isActive, page, pageSize);
        }

        public async Task<int> InsertEmailAccount(int companyId, EmailAccount emailAccount)
        {
            return await emailAccountRepository.InsertEmailAccount(companyId, emailAccount);
        }

        public async Task<bool> UpdateEmailAccount(int companyId, int id, EmailAccount emailAccount)
        {
            int affected = await emailAccountRepository.UpdateEmailAccount(companyId, id, emailAccount);
            return affected > 0;
        }

        public async Task<bool> DeleteEmailAccount(int companyId, int id)
        {
            int affected = await emailAccountRepository.DeleteEmailAccount(companyId, id);
            return affected > 0;
        }
    }
}
