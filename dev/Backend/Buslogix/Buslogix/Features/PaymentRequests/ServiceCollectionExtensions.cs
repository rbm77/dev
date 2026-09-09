using Buslogix.Features.Matching.Abstractions;

namespace Buslogix.Features.PaymentRequests
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the PaymentRequests feature: the payment request
        /// repository and service, and the trigger queue behind the
        /// "auto-approve" endpoint - its TriggerWorker is wired up in
        /// Program.cs, since it needs the AutoApprovePaymentRequests
        /// delegate composed there. PaymentRequestService depends on
        /// IPaymentMatchQueue (Features/Matching) - AddPaymentMatching()
        /// must also be called, in either order.
        /// </summary>
        public static IServiceCollection AddPaymentRequests(this IServiceCollection services)
        {
            services.AddScoped<IPaymentRequestRepository, PaymentRequestRepository>();
            services.AddScoped<IPaymentRequestService, PaymentRequestService>();
            services.AddSingleton<PaymentAutoApprovalQueue>();

            return services;
        }
    }
}
