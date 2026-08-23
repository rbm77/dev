namespace Buslogix.Models.DTO
{
    public class QrCodeResponseItem
    {
        public string? Description { get; set; }
        public string QrCodeBase64 { get; set; } = string.Empty;

        public QrCodeResponseItem() { }
    }
}
