
namespace Buslogix.Features.WhatsAppAccounts
{
    public interface IWhatsAppAccountRepository
    {

        Task<PagedResult<WhatsAppAccount>> GetWhatsAppAccounts(int? companyId, bool? isActive, int page, int pageSize);

        Task<WhatsAppAccount?> GetWhatsAppAccountByPhoneNumberId(string phoneNumberId);

        Task<int> InsertWhatsAppAccount(int companyId, WhatsAppAccount whatsAppAccount);

        Task<int> UpdateWhatsAppAccount(int companyId, int id, WhatsAppAccount whatsAppAccount);

        Task<int> DeleteWhatsAppAccount(int companyId, int id);
    }
}
