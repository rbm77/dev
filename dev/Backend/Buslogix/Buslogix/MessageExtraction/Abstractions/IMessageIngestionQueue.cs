using Buslogix.MessageExtraction.Queue;

namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// In-memory hand-off point between message producers (EmailIngestion's
    /// worker, the SMS controller) and the extraction worker that consumes
    /// them. Enqueueing never waits for extraction to happen - it only hands
    /// the item off.
    /// </summary>
    public interface IMessageIngestionQueue
    {
        ValueTask EnqueueAsync(IngestionItem item, CancellationToken ct = default);

        IAsyncEnumerable<IngestionItem> DequeueAllAsync(CancellationToken ct);
    }
}
