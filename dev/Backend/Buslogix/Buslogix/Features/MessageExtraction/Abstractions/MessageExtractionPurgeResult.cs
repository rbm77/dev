namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public record MessageExtractionPurgeResult(int FailuresDeletedCount, int ResultsDeletedCount);
}
