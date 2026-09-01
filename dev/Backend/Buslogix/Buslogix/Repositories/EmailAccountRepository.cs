using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class EmailAccountRepository(IDataAccess dataAccess) : IEmailAccountRepository
    {
        public async Task<PagedResult<EmailAccount>> GetEmailAccounts(int? companyId, bool? isActive, int page, int pageSize)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_is_active"] = isActive,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<EmailAccount> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_email_accounts", CommandType.StoredProcedure,
                static reader => new EmailAccount
                {
                    CompanyId = reader.GetInt32OrDefault(0),
                    Id = reader.GetInt32OrDefault(1),
                    EmailAddress = reader.GetStringOrDefault(2),
                    AppPassword = reader.GetStringOrDefault(3),
                    ImapHost = reader.GetStringOrDefault(4),
                    ImapPort = reader.GetInt32OrDefault(5),
                    IsActive = reader.GetBooleanOrDefault(6),
                    LastCheckedAt = reader.GetDateTimeOrDefault(7)
                }, parameters);

            return new PagedResult<EmailAccount>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<int> InsertEmailAccount(int companyId, EmailAccount emailAccount)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_email_address"] = emailAccount.EmailAddress,
                ["p_app_password"] = emailAccount.AppPassword,
                ["p_imap_host"] = emailAccount.ImapHost,
                ["p_imap_port"] = emailAccount.ImapPort,
                ["p_is_active"] = emailAccount.IsActive
            };

            object? result = await dataAccess.ExecuteScalar("insert_email_account", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateEmailAccount(int companyId, int id, EmailAccount emailAccount)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_email_address"] = emailAccount.EmailAddress,
                ["p_app_password"] = emailAccount.AppPassword,
                ["p_imap_host"] = emailAccount.ImapHost,
                ["p_imap_port"] = emailAccount.ImapPort,
                ["p_is_active"] = emailAccount.IsActive
            };

            return await dataAccess.ExecuteNonQuery("update_email_account", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteEmailAccount(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_email_account", CommandType.StoredProcedure, parameters);
        }

        public async Task UpdateLastChecked(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            await dataAccess.ExecuteNonQuery("update_email_account_last_checked", CommandType.StoredProcedure, parameters);
        }
    }
}
