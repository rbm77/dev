namespace Buslogix.Features.MaintenanceExpenses
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMaintenanceExpenses(this IServiceCollection services)
        {
            services.AddScoped<IMaintenanceExpenseRepository, MaintenanceExpenseRepository>();

            return services;
        }
    }
}
