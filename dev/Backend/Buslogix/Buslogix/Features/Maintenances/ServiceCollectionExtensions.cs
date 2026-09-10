namespace Buslogix.Features.Maintenances
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMaintenances(this IServiceCollection services)
        {
            services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

            return services;
        }
    }
}
