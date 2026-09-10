
namespace Buslogix.Features.EmailSenders
{
    public interface IEmailSenderRepository
    {

        Task<PagedResult<EmailSender>> GetEmailSenders(int? companyId, bool? isActive, int page, int pageSize);

        Task<int> InsertEmailSender(int companyId, EmailSender emailSender);

        Task<int> UpdateEmailSender(int companyId, int id, EmailSender emailSender);

        Task<int> DeleteEmailSender(int companyId, int id);
    }
}
