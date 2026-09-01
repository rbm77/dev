using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IEmailSenderRepository
    {

        /// <summary>Pass companyId: null and isActive: true for a cross-tenant, active-only fetch (the polling job).</summary>
        Task<PagedResult<EmailSender>> GetEmailSenders(int? companyId, bool? isActive, int page, int pageSize);

        Task<int> InsertEmailSender(int companyId, EmailSender emailSender);

        Task<int> UpdateEmailSender(int companyId, int id, EmailSender emailSender);

        Task<int> DeleteEmailSender(int companyId, int id);
    }
}
