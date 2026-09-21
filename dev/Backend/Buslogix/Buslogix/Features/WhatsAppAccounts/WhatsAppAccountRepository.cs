using System.Data;

namespace Buslogix.Features.WhatsAppAccounts
{
    public class WhatsAppAccountRepository(IDataAccess dataAccess) : IWhatsAppAccountRepository
    {
        public async Task<PagedResult<WhatsAppAccount>> GetWhatsAppAccounts(int? companyId, bool? isActive, int page, int pageSize)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_is_active"] = isActive,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<WhatsAppAccount> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_whatsapp_accounts", CommandType.StoredProcedure,
                MapAccount, parameters);

            return new PagedResult<WhatsAppAccount>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<WhatsAppAccount?> GetWhatsAppAccountByPhoneNumberId(string phoneNumberId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_phone_number_id"] = phoneNumberId
            };

            List<WhatsAppAccount> rows = await dataAccess.ExecuteReader("get_whatsapp_account_by_phone_number_id", CommandType.StoredProcedure,
                MapAccount, parameters);

            return rows.FirstOrDefault();
        }

        public async Task<int> InsertWhatsAppAccount(int companyId, WhatsAppAccount whatsAppAccount)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_phone_number_id"] = whatsAppAccount.PhoneNumberId,
                ["p_access_token"] = whatsAppAccount.AccessToken,
                ["p_is_active"] = whatsAppAccount.IsActive
            };

            object? result = await dataAccess.ExecuteScalar("insert_whatsapp_account", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateWhatsAppAccount(int companyId, int id, WhatsAppAccount whatsAppAccount)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_phone_number_id"] = whatsAppAccount.PhoneNumberId,
                ["p_access_token"] = whatsAppAccount.AccessToken,
                ["p_is_active"] = whatsAppAccount.IsActive
            };

            return await dataAccess.ExecuteNonQuery("update_whatsapp_account", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteWhatsAppAccount(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_whatsapp_account", CommandType.StoredProcedure, parameters);
        }

        private static WhatsAppAccount MapAccount(IDataReader reader) => new()
        {
            CompanyId = reader.GetInt32OrDefault(0),
            Id = reader.GetInt32OrDefault(1),
            PhoneNumberId = reader.GetStringOrDefault(2),
            AccessToken = reader.GetStringOrDefault(3),
            IsActive = reader.GetBooleanOrDefault(4)
        };
    }
}
