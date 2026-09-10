using System.Threading.Channels;
using Buslogix.Features.MessageExtraction.Abstractions;

namespace Buslogix.Features.MessageExtraction.Ingestion
{
    public class MessageIngestionQueue : IMessageIngestionQueue
    {
        private readonly Channel<IngestionItem> channel = Channel.CreateUnbounded<IngestionItem>();

        public ValueTask Enqueue(IngestionItem item, CancellationToken ct = default) =>
            channel.Writer.WriteAsync(item, ct);

        public IAsyncEnumerable<IngestionItem> DequeueAll(CancellationToken ct) =>
            channel.Reader.ReadAllAsync(ct);
    }
}
