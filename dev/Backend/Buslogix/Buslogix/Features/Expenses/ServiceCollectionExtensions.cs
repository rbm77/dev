namespace Buslogix.Features.Expenses
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Expenses feature: the expense repository.</summary>
        public static IServiceCollection AddExpenses(this IServiceCollection services)
        {
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            return services;
        }
    }
}
