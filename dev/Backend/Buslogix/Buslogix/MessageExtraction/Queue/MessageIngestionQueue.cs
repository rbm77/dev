using System.Threading.Channels;
using Buslogix.MessageExtraction.Abstractions;

namespace Buslogix.MessageExtraction.Queue
{
    /// <summary>
    /// Unbounded in-memory queue backing IMessageIngestionQueue. Unbounded
    /// because a producer (the email worker, the SMS controller) must never
    /// be made to wait on this - each item is a short raw-text string, so
    /// holding many of them briefly is cheap. Registered as a singleton: one
    /// shared instance for the whole app.
    /// </summary>
    public class MessageIngestionQueue : IMessageIngestionQueue
    {
        private readonly Channel<IngestionItem> channel = Channel.CreateUnbounded<IngestionItem>();

        public ValueTask EnqueueAsync(IngestionItem item, CancellationToken ct = default) =>
            channel.Writer.WriteAsync(item, ct);

        public IAsyncEnumerable<IngestionItem> DequeueAllAsync(CancellationToken ct) =>
            channel.Reader.ReadAllAsync(ct);
    }
}
