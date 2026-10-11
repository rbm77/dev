using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Common.BackgroundJobs
{
    public class TriggerWorker(
        TriggerQueue queue,
        IServiceScopeFactory scopeFactory,
        Func<IServiceProvider, CancellationToken, Task> work,
        ILogHandler logHandler) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (byte _ in queue.DequeueAll(stoppingToken))
            {
                using IServiceScope scope = scopeFactory.CreateScope();
                using IDisposable logScope = LogScope.Push(LogScope.NewId(), null);

                try
                {
                    await work(scope.ServiceProvider, stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    await logHandler.WriteLog($"Trigger worker error: {ex.Message}", LogType.Error);
                }
            }
        }
    }
}
