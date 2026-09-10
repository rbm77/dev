namespace Buslogix.Features.Payments
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPayments(this IServiceCollection services)
        {
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }
    }
}
