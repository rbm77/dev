namespace Buslogix.Features.MaintenanceExpenses
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the MaintenanceExpenses feature: the maintenance expense repository.</summary>
        public static IServiceCollection AddMaintenanceExpenses(this IServiceCollection services)
        {
            services.AddScoped<IMaintenanceExpenseRepository, MaintenanceExpenseRepository>();

            return services;
        }
    }
}
