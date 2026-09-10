namespace Buslogix.Features.Companies
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCompanies(this IServiceCollection services)
        {
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            return services;
        }
    }
}
