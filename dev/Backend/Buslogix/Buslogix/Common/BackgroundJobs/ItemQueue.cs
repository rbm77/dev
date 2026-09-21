using System.Threading.Channels;

namespace Buslogix.Common.BackgroundJobs
{
    public class ItemQueue<T> : IItemQueue<T>
    {
        private readonly Channel<T> channel = Channel.CreateUnbounded<T>();

        public ValueTask Enqueue(T item, CancellationToken ct = default) =>
            channel.Writer.WriteAsync(item, ct);

        public IAsyncEnumerable<T> DequeueAll(CancellationToken ct) =>
            channel.Reader.ReadAllAsync(ct);
    }
}
