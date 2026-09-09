using Buslogix.Common.BackgroundJobs;

namespace Buslogix.Features.EmailIngestion
{
    /// <summary>Trigger for EmailIngestionController's "poll" endpoint.</summary>
    public class EmailPollQueue : TriggerQueue;
}
