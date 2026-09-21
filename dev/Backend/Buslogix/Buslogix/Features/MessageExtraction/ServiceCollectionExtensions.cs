using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Configuration;
using Buslogix.Features.MessageExtraction.Llm;
using Buslogix.Features.MessageExtraction.Parsers;
using Buslogix.Features.MessageExtraction.Persistence;
using Buslogix.Features.MessageExtraction.Ingestion;

namespace Buslogix.Features.MessageExtraction
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageExtraction(this IServiceCollection services)
        {
            services.AddOptions<ExtractionPatternsOptions>().BindConfiguration("ExtractionPatterns");
            services.AddOptions<LlmFallbackOptions>().BindConfiguration("LlmFallback");

            services.AddSingleton<IPatternProvider, PatternProvider>();
            services.AddSingleton<IMessageParser, ConfigDrivenRegexParser>();

            services.AddScoped<ILlmExtractionFallback, LlmExtractionFallback>();
            services.AddScoped<IMessageExtractionService, MessageExtractor>();

            services.AddSingleton<IMessageIngestionQueue, MessageIngestionQueue>();
            services.AddScoped<IMessageExtractionFailureRepository, MessageExtractionFailureRepository>();
            services.AddScoped<IMessageExtractionResultRepository, MessageExtractionResultRepository>();
            services.AddHostedService<MessageExtractionWorker>();

            services.AddScoped<IMessageExtractionHistoryRepository, MessageExtractionHistoryRepository>();
            services.AddScoped<IMessageExtractionMaintenanceService, MessageExtractionMaintenanceService>();
            services.AddSingleton<MessageExtractionRetryQueue>();
            services.AddSingleton<MessageExtractionPurgeQueue>();

            return services;
        }
    }
}
