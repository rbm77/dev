namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public record ExtractedData(decimal Amount, string Reference, DateTime? Date);
}
