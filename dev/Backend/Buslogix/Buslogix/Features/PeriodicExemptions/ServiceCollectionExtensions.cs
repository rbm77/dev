namespace Buslogix.Features.PeriodicExemptions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the PeriodicExemptions feature: the periodic exemption repository.</summary>
        public static IServiceCollection AddPeriodicExemptions(this IServiceCollection services)
        {
            services.AddScoped<IPeriodicExemptionRepository, PeriodicExemptionRepository>();

            return services;
        }
    }
}
