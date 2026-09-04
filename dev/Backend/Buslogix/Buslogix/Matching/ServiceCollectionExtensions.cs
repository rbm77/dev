using Buslogix.Matching.Abstractions;
using Buslogix.Matching.Persistence;

namespace Buslogix.Matching
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the payment/extraction matching feature: the in-memory
        /// queue that MessageExtractionWorker and PaymentRequestService push
        /// to right after their own inserts succeed, the repository that
        /// calls the matching stored procedures, and the background worker
        /// that consumes the queue.
        /// </summary>
        public static IServiceCollection AddPaymentMatching(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentMatchQueue, PaymentMatchQueue>();
            services.AddScoped<IPaymentMatchingRepository, PaymentMatchingRepository>();
            services.AddHostedService<PaymentMatchWorker>();

            return services;
        }
    }
}
