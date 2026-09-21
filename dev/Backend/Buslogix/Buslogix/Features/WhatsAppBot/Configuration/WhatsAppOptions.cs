namespace Buslogix.Features.WhatsAppBot.Configuration
{
    public class WhatsAppOptions
    {
        public string VerifyToken { get; set; } = string.Empty;

        public string AppSecret { get; set; } = string.Empty;

        public string ApiVersion { get; set; } = "v21.0";
    }
}
