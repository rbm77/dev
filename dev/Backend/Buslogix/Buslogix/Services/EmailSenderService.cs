using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class EmailSenderService(IEmailSenderRepository emailSenderRepository) : IEmailSenderService
    {

        public async Task<PagedResult<EmailSender>> GetEmailSenders(int companyId, bool? isActive, int page, int pageSize)
        {
            return await emailSenderRepository.GetEmailSenders(companyId, isActive, page, pageSize);
        }

        public async Task<int> InsertEmailSender(int companyId, EmailSender emailSender)
        {
            return await emailSenderRepository.InsertEmailSender(companyId, emailSender);
        }

        public async Task<bool> UpdateEmailSender(int companyId, int id, EmailSender emailSender)
        {
            int affected = await emailSenderRepository.UpdateEmailSender(companyId, id, emailSender);
            return affected > 0;
        }

        public async Task<bool> DeleteEmailSender(int companyId, int id)
        {
            int affected = await emailSenderRepository.DeleteEmailSender(companyId, id);
            return affected > 0;
        }
    }
}
