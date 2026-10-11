using Buslogix.Common.Llm;
using Buslogix.Features.WhatsAppBot.Abstractions;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.WhatsAppBot.ImageExtraction
{
    public class WhatsAppReceiptProcessor(
        IWhatsAppGraphClient graphClient,
        IWhatsAppReceiptImageExtractor extractor,
        IPaymentRequestService paymentRequestService,
        ILogHandler logHandler) : IWhatsAppReceiptProcessor
    {
        public async Task Process(WhatsAppImageExtractionItem item, CancellationToken ct)
        {
            (byte[] bytes, string mimeType) = await graphClient.DownloadMedia(item.AccessToken, item.MediaId, ct);
            ExtractedData? extracted = await extractor.Extract(bytes, mimeType, ct);

            if (extracted is null)
            {
                await logHandler.WriteLog($"WhatsApp receipt extraction: could not parse LLM response for WaId {item.WaId}.", LogType.Warning);
                await graphClient.SendTextMessage(
                    item.AccessToken, item.PhoneNumberId, item.WaId,
                    "No pudimos leer los datos de tu comprobante 😕 Escribinos de nuevo para intentarlo otra vez.", ct);
                return;
            }

            PaymentRequest paymentRequest = new()
            {
                StudentId = item.StudentId,
                Amount = extracted.Amount,
                ReceiptReference = extracted.Reference,
                Date = extracted.Date
            };

            int id = await paymentRequestService.InsertPaymentRequest(item.CompanyId, paymentRequest);

            await graphClient.SendTextMessage(
                item.AccessToken, item.PhoneNumberId, item.WaId,
                id > 0
                    ? "¡Listo! Registramos tu comprobante de pago correctamente 🎉"
                    : "No pudimos registrar tu comprobante de pago 😕 Por favor comunicate con un colaborador.", ct);
        }
    }
}
