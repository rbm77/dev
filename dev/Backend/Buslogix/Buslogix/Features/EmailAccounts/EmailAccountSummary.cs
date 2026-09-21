namespace Buslogix.Features.EmailAccounts
{
    public class EmailAccountSummary
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string? EmailAddress { get; set; }
        public string? ImapHost { get; set; }
        public int ImapPort { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastCheckedAt { get; set; }

        public static EmailAccountSummary FromAccount(EmailAccount account) => new()
        {
            CompanyId = account.CompanyId,
            Id = account.Id,
            EmailAddress = account.EmailAddress,
            ImapHost = account.ImapHost,
            ImapPort = account.ImapPort,
            IsActive = account.IsActive,
            LastCheckedAt = account.LastCheckedAt
        };
    }
}
