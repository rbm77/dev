namespace Buslogix.Features.SpecificExemptions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the SpecificExemptions feature: the specific exemption repository.</summary>
        public static IServiceCollection AddSpecificExemptions(this IServiceCollection services)
        {
            services.AddScoped<ISpecificExemptionRepository, SpecificExemptionRepository>();

            return services;
        }
    }
}
