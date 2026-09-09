namespace Buslogix.Features.Incidents
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Incidents feature: the incident repository.</summary>
        public static IServiceCollection AddIncidents(this IServiceCollection services)
        {
            services.AddScoped<IIncidentRepository, IncidentRepository>();

            return services;
        }
    }
}
