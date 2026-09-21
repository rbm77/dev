namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppSignatureValidator
    {
        bool IsValid(byte[] payload, string? signatureHeader);
    }
}
