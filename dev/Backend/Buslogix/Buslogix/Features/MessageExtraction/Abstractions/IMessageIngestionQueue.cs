using Buslogix.Features.MessageExtraction.Ingestion;

namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageIngestionQueue
    {
        ValueTask Enqueue(IngestionItem item, CancellationToken ct = default);

        IAsyncEnumerable<IngestionItem> DequeueAll(CancellationToken ct);
    }
}
