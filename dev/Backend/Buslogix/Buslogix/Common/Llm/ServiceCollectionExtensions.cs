using Anthropic;

namespace Buslogix.Common.Llm
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClaudeChatCompletion(this IServiceCollection services)
        {
            services.AddOptions<ClaudeOptions>().BindConfiguration("LlmFallback");

            services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
                string apiKey = configuration["LlmFallback:ApiKey"]
                    ?? throw new InvalidOperationException(
                        "Configuration key 'LlmFallback:ApiKey' is not set. In development: " +
                        "dotnet user-secrets set \"LlmFallback:ApiKey\" \"<key>\" --project Buslogix.csproj. " +
                        "In production: set the environment variable LlmFallback__ApiKey.");

                return new AnthropicClient { ApiKey = apiKey };
            });

            services.AddSingleton<IChatCompletionClient, ClaudeChatCompletionClient>();

            return services;
        }
    }
}
