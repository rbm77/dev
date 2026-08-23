using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IQrCodeService
    {
        List<QrCodeResponseItem> GenerateQrCodes(List<QrCodeRequestItem> items, int size);
    }
}
