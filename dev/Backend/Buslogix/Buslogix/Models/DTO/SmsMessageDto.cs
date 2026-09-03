namespace Buslogix.Models.DTO
{
    public class SmsMessageDto
    {
        public int CompanyId { get; set; }
        public string MessageText { get; set; } = string.Empty;
    }
}
