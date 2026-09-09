
namespace Buslogix.Features.Students
{
    public interface IQrCodeService
    {
        List<QrCodeResponseItem> GenerateQrCodes(List<QrCodeRequestItem> items, int size);
    }
}
