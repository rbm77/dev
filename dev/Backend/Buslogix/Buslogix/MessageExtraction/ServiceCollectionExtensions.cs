using Anthropic;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.MessageExtraction.Configuration;
using Buslogix.MessageExtraction.Llm;
using Buslogix.MessageExtraction.Parsers;
using Buslogix.MessageExtraction.Persistence;
using Buslogix.MessageExtraction.Queue;

namespace Buslogix.MessageExtraction
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the whole MessageExtraction feature: options binding, the Tier 1
        /// config-driven regex parser, the Tier 2 LLM fallback (backed by the
        /// Anthropic SDK), and the orchestrator.
        /// </summary>
        public static IServiceCollection AddMessageExtraction(this IServiceCollection services)
        {
            services.AddOptions<ExtractionPatternsOptions>().BindConfiguration("ExtractionPatterns");
            services.AddOptions<LlmFallbackOptions>().BindConfiguration("LlmFallback");

            services.AddSingleton<IPatternProvider, PatternProvider>();
            services.AddSingleton<IMessageParser, ConfigDrivenRegexParser>();

            // ApiKey is resolved directly from IConfiguration (never bound to
            // LlmFallbackOptions) so the secret can't end up in an {@Options} log
            // dump. Dev: dotnet user-secrets. Prod: env var LlmFallback__ApiKey.
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

            services.AddScoped<ILlmExtractionFallback, LlmExtractionFallback>();
            services.AddScoped<IMessageExtractionService, MessageExtractor>();

            // Async ingestion queue: producers (EmailIngestion's worker, the
            // SMS controller) enqueue raw text here without waiting for
            // extraction; MessageExtractionWorker consumes it in the
            // background, regardless of source.
            services.AddSingleton<IMessageIngestionQueue, MessageIngestionQueue>();
            services.AddScoped<IMessageExtractionFailureRepository, MessageExtractionFailureRepository>();
            services.AddScoped<IMessageExtractionResultRepository, MessageExtractionResultRepository>();
            services.AddHostedService<MessageExtractionWorker>();

            // Housekeeping behind the retry-failures/purge-history trigger
            // endpoints (see MessageExtractionController and the two
            // TriggerWorker registrations in Program.cs).
            services.AddScoped<IMessageExtractionHistoryRepository, MessageExtractionHistoryRepository>();
            services.AddScoped<IMessageExtractionMaintenanceService, MessageExtractionMaintenanceService>();

            return services;
        }
    }
}
