namespace Buslogix.Features.WhatsAppAccounts
{
    public class WhatsAppAccountSummary
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string? PhoneNumberId { get; set; }
        public bool IsActive { get; set; }

        public static WhatsAppAccountSummary FromAccount(WhatsAppAccount account) => new()
        {
            CompanyId = account.CompanyId,
            Id = account.Id,
            PhoneNumberId = account.PhoneNumberId,
            IsActive = account.IsActive
        };
    }
}
