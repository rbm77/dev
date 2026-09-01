using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IEmailSenderService
    {

        Task<PagedResult<EmailSender>> GetEmailSenders(int companyId, bool? isActive, int page, int pageSize);

        Task<int> InsertEmailSender(int companyId, EmailSender emailSender);

        Task<bool> UpdateEmailSender(int companyId, int id, EmailSender emailSender);

        Task<bool> DeleteEmailSender(int companyId, int id);
    }
}
