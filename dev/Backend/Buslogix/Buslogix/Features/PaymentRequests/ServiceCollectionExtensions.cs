using Buslogix.Features.PaymentMatching.Abstractions;

namespace Buslogix.Features.PaymentRequests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentRequests(this IServiceCollection services)
        {
            services.AddScoped<IPaymentRequestRepository, PaymentRequestRepository>();
            services.AddScoped<IPaymentRequestService, PaymentRequestService>();
            services.AddSingleton<PaymentAutoApprovalQueue>();

            return services;
        }
    }
}
