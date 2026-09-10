namespace Buslogix.Features.PeriodicExemptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPeriodicExemptions(this IServiceCollection services)
        {
            services.AddScoped<IPeriodicExemptionRepository, PeriodicExemptionRepository>();

            return services;
        }
    }
}
