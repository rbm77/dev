namespace Buslogix.Models
{
    public class EmailSender
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string? SenderAddress { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public EmailSender() { }
    }
}
