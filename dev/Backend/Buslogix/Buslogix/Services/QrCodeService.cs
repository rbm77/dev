using System.Text;
using Buslogix.Interfaces;
using Buslogix.Models.DTO;
using QRCoder;

namespace Buslogix.Services
{
    public class QrCodeService : IQrCodeService
    {
        private const int DefaultSize = 20;
        private const int MaxSize = 50;
        private const int MaxItems = 20;
        private const int MaxValueBytes = 300;

        public List<QrCodeResponseItem> GenerateQrCodes(List<QrCodeRequestItem> items, int size)
        {
            if (size <= 0 || size > MaxSize)
            {
                size = DefaultSize;
            }

            using QRCodeGenerator qrCodeGenerator = new();

            List<QrCodeResponseItem> results = [];

            foreach (QrCodeRequestItem item in items.Take(MaxItems))
            {
                if (string.IsNullOrEmpty(item.Value) || Encoding.UTF8.GetByteCount(item.Value) > MaxValueBytes)
                {
                    continue;
                }

                using QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(item.Value, QRCodeGenerator.ECCLevel.Q);
                using PngByteQRCode pngQrCode = new(qrCodeData);
                byte[] qrCodeBytes = pngQrCode.GetGraphic(size);

                results.Add(new QrCodeResponseItem
                {
                    Description = item.Description,
                    QrCodeBase64 = Convert.ToBase64String(qrCodeBytes)
                });
            }

            return results;
        }
    }
}
