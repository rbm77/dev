using Buslogix.Interfaces;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Triggers
{
    /// <summary>
    /// Generic background runner for a TriggerQueue: a single consumer picks
    /// up each trigger, runs `work` in its own DI scope (independent of any
    /// HTTP request scope), and logs/swallows any exception so one failed run
    /// never stops the loop from picking up the next trigger. Because the
    /// queue it reads from has capacity 1, at most one run of `work` is ever
    /// pending, and the single consumer here guarantees runs never overlap.
    /// One instance is created per trigger (email poll, payment-period
    /// schedule, payment auto-approval, ...), each wired to its own queue and
    /// its own `work` delegate in Program.cs.
    /// </summary>
    public class TriggerWorker(
        TriggerQueue queue,
        IServiceScopeFactory scopeFactory,
        Func<IServiceProvider, CancellationToken, Task> work,
        ILogHandler logHandler) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (byte _ in queue.DequeueAllAsync(stoppingToken))
            {
                using IServiceScope scope = scopeFactory.CreateScope();

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
