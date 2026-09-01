namespace Buslogix.Models
{
    public class EmailAccount
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string? EmailAddress { get; set; }
        public string? AppPassword { get; set; }
        public string? ImapHost { get; set; }
        public int ImapPort { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastCheckedAt { get; set; }

        public EmailAccount() { }
    }
}
