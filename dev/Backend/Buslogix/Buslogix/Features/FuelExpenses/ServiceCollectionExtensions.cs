namespace Buslogix.Features.FuelExpenses
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the FuelExpenses feature: the fuel expense repository.</summary>
        public static IServiceCollection AddFuelExpenses(this IServiceCollection services)
        {
            services.AddScoped<IFuelExpenseRepository, FuelExpenseRepository>();

            return services;
        }
    }
}
