namespace Buslogix.Features.PaymentPeriods
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the PaymentPeriods feature: the payment period
        /// repository and the trigger queue behind the
        /// "schedule-payment-periods" endpoint - its TriggerWorker is wired
        /// up in Program.cs, since it needs the SchedulePaymentPeriods
        /// delegate composed there.
        /// </summary>
        public static IServiceCollection AddPaymentPeriods(this IServiceCollection services)
        {
            services.AddScoped<IPaymentPeriodRepository, PaymentPeriodRepository>();
            services.AddSingleton<PaymentPeriodScheduleQueue>();

            return services;
        }
    }
}
