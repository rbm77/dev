namespace Buslogix.Features.Users
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUsers(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenHandler, TokenHandler>();

            return services;
        }
    }
}
