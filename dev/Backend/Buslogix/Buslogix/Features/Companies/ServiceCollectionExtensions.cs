namespace Buslogix.Features.Companies
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Companies feature: the company repository.</summary>
        public static IServiceCollection AddCompanies(this IServiceCollection services)
        {
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            return services;
        }
    }
}
