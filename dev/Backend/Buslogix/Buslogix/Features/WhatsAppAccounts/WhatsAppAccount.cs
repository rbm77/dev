namespace Buslogix.Features.WhatsAppAccounts
{
    public class WhatsAppAccount
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string? PhoneNumberId { get; set; }
        public string? AccessToken { get; set; }
        public bool IsActive { get; set; }

        public WhatsAppAccount() { }
    }
}
