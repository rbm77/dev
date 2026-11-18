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

            string? reference = ExtractedData.NormalizeReference(extracted.Reference);

            PaymentRequest paymentRequest = new()
            {
                StudentId = item.StudentId,
                Amount = extracted.Amount,
                ReceiptReference = reference,
                Date = extracted.Date
            };

            int id = await paymentRequestService.InsertPaymentRequest(item.CompanyId, paymentRequest);

            string reply = id <= 0
                ? "No pudimos registrar tu comprobante de pago 😕 Por favor comunicate con un colaborador."
                : reference is null
                    ? "¡Listo! Registramos tu comprobante de pago 🎉 No encontramos un número de referencia, así que un colaborador lo revisará manualmente."
                    : "¡Listo! Registramos tu comprobante de pago correctamente 🎉";

            await graphClient.SendTextMessage(item.AccessToken, item.PhoneNumberId, item.WaId, reply, ct);
        }
    }
}
