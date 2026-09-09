namespace Buslogix.Features.MessageExtraction
{
    public class SmsMessageDto
    {
        public int CompanyId { get; set; }
        public string MessageText { get; set; } = string.Empty;
    }
}
