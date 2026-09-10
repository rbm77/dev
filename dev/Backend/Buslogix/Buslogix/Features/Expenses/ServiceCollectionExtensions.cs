namespace Buslogix.Features.Expenses
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExpenses(this IServiceCollection services)
        {
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            return services;
        }
    }
}
