using static Buslogix.Utilities.Enums;

namespace Buslogix.MessageExtraction.Queue
{
    /// <summary>
    /// A raw message handed off to the extraction pipeline, regardless of
    /// where it came from (email or SMS). Source is carried only for logging
    /// context - it does not affect extraction and is not persisted.
    /// </summary>
    public record IngestionItem(IngestionSource Source, int CompanyId, string RawText, DateTime ReceivedAt);
}
