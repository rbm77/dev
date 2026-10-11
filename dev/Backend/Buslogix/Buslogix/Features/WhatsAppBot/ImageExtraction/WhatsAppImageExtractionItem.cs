namespace Buslogix.Features.WhatsAppBot.ImageExtraction
{
    public record WhatsAppImageExtractionItem(
        int CompanyId,
        string PhoneNumberId,
        string AccessToken,
        string WaId,
        int StudentId,
        string MediaId) : ICompanyScoped;
}
