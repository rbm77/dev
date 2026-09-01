using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class EmailSenderRepository(IDataAccess dataAccess) : IEmailSenderRepository
    {

        public async Task<PagedResult<EmailSender>> GetEmailSenders(int? companyId, bool? isActive, int page, int pageSize)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_is_active"] = isActive,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<EmailSender> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_email_senders", CommandType.StoredProcedure,
                static reader => new EmailSender
                {
                    CompanyId = reader.GetInt32OrDefault(0),
                    Id = reader.GetInt32OrDefault(1),
                    SenderAddress = reader.GetStringOrDefault(2),
                    Description = reader.GetStringOrDefault(3),
                    IsActive = reader.GetBooleanOrDefault(4)
                }, parameters);

            return new PagedResult<EmailSender>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<int> InsertEmailSender(int companyId, EmailSender emailSender)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_sender_address"] = emailSender.SenderAddress,
                ["p_description"] = emailSender.Description,
                ["p_is_active"] = emailSender.IsActive
            };

            object? result = await dataAccess.ExecuteScalar("insert_email_sender", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateEmailSender(int companyId, int id, EmailSender emailSender)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_sender_address"] = emailSender.SenderAddress,
                ["p_description"] = emailSender.Description,
                ["p_is_active"] = emailSender.IsActive
            };

            return await dataAccess.ExecuteNonQuery("update_email_sender", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteEmailSender(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_email_sender", CommandType.StoredProcedure, parameters);
        }
    }
}
