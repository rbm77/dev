namespace Buslogix.Features.FuelExpenses
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFuelExpenses(this IServiceCollection services)
        {
            services.AddScoped<IFuelExpenseRepository, FuelExpenseRepository>();

            return services;
        }
    }
}
