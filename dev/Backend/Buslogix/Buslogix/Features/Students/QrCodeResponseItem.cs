namespace Buslogix.Features.Students
{
    public class QrCodeResponseItem
    {
        public string? Description { get; set; }
        public string QrCodeBase64 { get; set; } = string.Empty;

        public QrCodeResponseItem() { }
    }
}
