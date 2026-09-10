namespace Buslogix.Features.IncidentExpenses
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIncidentExpenses(this IServiceCollection services)
        {
            services.AddScoped<IIncidentExpenseRepository, IncidentExpenseRepository>();

            return services;
        }
    }
}
