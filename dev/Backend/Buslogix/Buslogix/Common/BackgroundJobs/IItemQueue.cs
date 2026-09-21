namespace Buslogix.Common.BackgroundJobs
{
    public interface IItemQueue<T>
    {
        ValueTask Enqueue(T item, CancellationToken ct = default);

        IAsyncEnumerable<T> DequeueAll(CancellationToken ct);
    }
}
