namespace Buslogix.Features.Roles
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRoles(this IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }
    }
}
