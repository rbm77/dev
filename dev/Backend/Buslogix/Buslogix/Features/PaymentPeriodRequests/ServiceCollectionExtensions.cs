namespace Buslogix.Features.PaymentPeriodRequests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentPeriodRequests(this IServiceCollection services)
        {
            services.AddScoped<IPaymentPeriodRequestRepository, PaymentPeriodRequestRepository>();

            return services;
        }
    }
}
