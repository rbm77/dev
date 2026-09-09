using Buslogix.Features.Matching.Abstractions;
using Buslogix.Features.Matching.Persistence;

namespace Buslogix.Features.Matching
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the payment/extraction matching feature: the in-memory
        /// queue that MessageExtractionWorker and PaymentRequestService push
        /// to right after their own inserts succeed, the repository that
        /// calls the matching stored procedures, the background worker that
        /// consumes the queue, and the trigger queue behind the
        /// "match-pending" sweep endpoint - its TriggerWorker is wired up in
        /// Program.cs, since it needs the MatchPendingPaymentRequests
        /// delegate composed there.
        /// </summary>
        public static IServiceCollection AddPaymentMatching(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentMatchQueue, PaymentMatchQueue>();
            services.AddScoped<IPaymentMatchingRepository, PaymentMatchingRepository>();
            services.AddHostedService<PaymentMatchWorker>();
            services.AddSingleton<PaymentMatchSweepQueue>();

            return services;
        }
    }
}
