namespace Buslogix.Features.SpecificExemptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSpecificExemptions(this IServiceCollection services)
        {
            services.AddScoped<ISpecificExemptionRepository, SpecificExemptionRepository>();

            return services;
        }
    }
}
