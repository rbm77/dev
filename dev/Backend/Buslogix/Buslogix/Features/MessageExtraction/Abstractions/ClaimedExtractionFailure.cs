namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public record ClaimedExtractionFailure(int CompanyId, string RawText, DateTime ReceivedAt);
}
