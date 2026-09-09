using Buslogix.Common.BackgroundJobs;

namespace Buslogix.Features.MessageExtraction
{
    /// <summary>Trigger for MessageExtractionController's "purge-history" endpoint.</summary>
    public class MessageExtractionPurgeQueue : TriggerQueue;
}
