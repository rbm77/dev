using Buslogix.Features.PaymentMatching.Abstractions;
using Buslogix.Features.PaymentMatching.Persistence;

namespace Buslogix.Features.PaymentMatching
{
    public static class ServiceCollectionExtensions
    {
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
