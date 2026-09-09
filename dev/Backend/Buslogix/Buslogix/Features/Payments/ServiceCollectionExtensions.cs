namespace Buslogix.Features.Payments
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Payments feature: the payment repository.</summary>
        public static IServiceCollection AddPayments(this IServiceCollection services)
        {
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }
    }
}
