namespace Buslogix.Models.DTO
{
    public class QrCodeRequestItem
    {
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }

        public QrCodeRequestItem() { }
    }
}
