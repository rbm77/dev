using System.Threading.Channels;

namespace Buslogix.Common.BackgroundJobs
{
    public class TriggerQueue
    {
        private readonly Channel<byte> channel = Channel.CreateBounded<byte>(
            new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite });

        public bool TryTrigger() => channel.Writer.TryWrite(0);

        public IAsyncEnumerable<byte> DequeueAll(CancellationToken ct) => channel.Reader.ReadAllAsync(ct);
    }
}
