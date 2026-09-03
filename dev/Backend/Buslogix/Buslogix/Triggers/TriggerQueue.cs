using System.Threading.Channels;

namespace Buslogix.Triggers
{
    /// <summary>
    /// A "run this now, in the background" signal with room for at most one
    /// pending trigger. Bounded to 1 with DropWrite: while a trigger is
    /// already queued (or being processed - it's out of the channel by
    /// then), any further TryTrigger() calls are silently ignored instead of
    /// piling up redundant runs. Concrete features derive a marker subclass
    /// (e.g. EmailPollQueue) so each gets its own independent instance/queue
    /// when registered as a singleton.
    /// </summary>
    public class TriggerQueue
    {
        private readonly Channel<byte> channel = Channel.CreateBounded<byte>(
            new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite });

        public bool TryTrigger() => channel.Writer.TryWrite(0);

        public IAsyncEnumerable<byte> DequeueAllAsync(CancellationToken ct) => channel.Reader.ReadAllAsync(ct);
    }
}
