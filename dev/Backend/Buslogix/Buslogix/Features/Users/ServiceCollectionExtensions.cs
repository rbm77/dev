namespace Buslogix.Features.Users
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Users feature: the user repository, the user service, and the JWT token handler.</summary>
        public static IServiceCollection AddUsers(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenHandler, TokenHandler>();

            return services;
        }
    }
}
