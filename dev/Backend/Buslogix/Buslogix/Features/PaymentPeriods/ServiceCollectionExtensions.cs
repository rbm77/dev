namespace Buslogix.Features.PaymentPeriods
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentPeriods(this IServiceCollection services)
        {
            services.AddScoped<IPaymentPeriodRepository, PaymentPeriodRepository>();
            services.AddSingleton<PaymentPeriodScheduleQueue>();

            return services;
        }
    }
}
