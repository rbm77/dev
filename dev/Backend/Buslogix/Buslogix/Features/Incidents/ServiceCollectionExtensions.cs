namespace Buslogix.Features.Incidents
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIncidents(this IServiceCollection services)
        {
            services.AddScoped<IIncidentRepository, IncidentRepository>();

            return services;
        }
    }
}
