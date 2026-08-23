namespace Buslogix.Models.DTO
{
    public class QrCodeGenerationRequest
    {
        public int Size { get; set; }
        public List<QrCodeRequestItem> Items { get; set; } = [];

        public QrCodeGenerationRequest() { }
    }
}
