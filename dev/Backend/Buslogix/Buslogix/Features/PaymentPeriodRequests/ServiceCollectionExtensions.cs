namespace Buslogix.Features.PaymentPeriodRequests
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the PaymentPeriodRequests feature: the payment period request repository.</summary>
        public static IServiceCollection AddPaymentPeriodRequests(this IServiceCollection services)
        {
            services.AddScoped<IPaymentPeriodRequestRepository, PaymentPeriodRequestRepository>();

            return services;
        }
    }
}
