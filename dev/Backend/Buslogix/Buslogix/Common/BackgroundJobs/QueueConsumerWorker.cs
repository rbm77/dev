using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Common.BackgroundJobs
{
    public abstract class QueueConsumerWorker<T>(
        IItemQueue<T> queue,
        IServiceScopeFactory scopeFactory,
        ILogHandler logHandler,
        int consumerCount = 1) : BackgroundService
    {
        protected ILogHandler LogHandler => logHandler;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IEnumerable<Task> consumers = Enumerable.Range(0, consumerCount)
                .Select(_ => Consume(stoppingToken));

            return Task.WhenAll(consumers);
        }

        private async Task Consume(CancellationToken ct)
        {
            await foreach (T item in queue.DequeueAll(ct))
            {
                using IServiceScope scope = scopeFactory.CreateScope();

                try
                {
                    await ProcessItem(scope.ServiceProvider, item, ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    await logHandler.WriteLog(DescribeError(item, ex), LogType.Error);
                }
            }
        }

        protected abstract Task ProcessItem(IServiceProvider sp, T item, CancellationToken ct);

        protected abstract string DescribeError(T item, Exception ex);
    }
}
