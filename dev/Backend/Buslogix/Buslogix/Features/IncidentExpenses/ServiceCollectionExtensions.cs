namespace Buslogix.Features.IncidentExpenses
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the IncidentExpenses feature: the incident expense repository.</summary>
        public static IServiceCollection AddIncidentExpenses(this IServiceCollection services)
        {
            services.AddScoped<IIncidentExpenseRepository, IncidentExpenseRepository>();

            return services;
        }
    }
}
