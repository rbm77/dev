namespace Buslogix.Features.Maintenances
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Maintenances feature: the maintenance repository.</summary>
        public static IServiceCollection AddMaintenances(this IServiceCollection services)
        {
            services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

            return services;
        }
    }
}
