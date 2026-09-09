using Buslogix.Common.BackgroundJobs;

namespace Buslogix.Features.MessageExtraction
{
    /// <summary>Trigger for MessageExtractionController's "retry-failures" endpoint.</summary>
    public class MessageExtractionRetryQueue : TriggerQueue;
}
