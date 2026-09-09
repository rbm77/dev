namespace Buslogix.Features.Roles
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Roles feature: the role repository.</summary>
        public static IServiceCollection AddRoles(this IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }
    }
}
