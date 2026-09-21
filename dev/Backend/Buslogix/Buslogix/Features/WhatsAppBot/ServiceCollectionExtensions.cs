using Buslogix.Features.WhatsAppBot.Abstractions;
using Buslogix.Features.WhatsAppBot.Configuration;
using Buslogix.Features.WhatsAppBot.Conversation;
using Buslogix.Features.WhatsAppBot.Graph;
using Buslogix.Features.WhatsAppBot.ImageExtraction;
using Buslogix.Features.WhatsAppBot.Llm;
using Buslogix.Features.WhatsAppBot.Webhook;
using Microsoft.Extensions.Options;

namespace Buslogix.Features.WhatsAppBot
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWhatsAppBot(this IServiceCollection services)
        {
            services.AddOptions<WhatsAppOptions>().BindConfiguration("WhatsApp");

            services.AddHttpClient<IWhatsAppGraphClient, WhatsAppGraphClient>((sp, client) =>
            {
                string apiVersion = sp.GetRequiredService<IOptionsMonitor<WhatsAppOptions>>().CurrentValue.ApiVersion;
                client.BaseAddress = new Uri($"https://graph.facebook.com/{apiVersion}/");
            });

            services.AddScoped<IWhatsAppSignatureValidator, WhatsAppSignatureValidator>();
            services.AddScoped<IWhatsAppReceiptImageExtractor, WhatsAppReceiptImageExtractor>();
            services.AddScoped<IWhatsAppReceiptProcessor, WhatsAppReceiptProcessor>();
            services.AddScoped<IWhatsAppConversationService, WhatsAppConversationService>();

            services.AddSingleton<IWhatsAppConversationStateStore, WhatsAppConversationStateStore>();
            services.AddSingleton<IWhatsAppConversationQueue, WhatsAppConversationQueue>();
            services.AddSingleton<IWhatsAppImageExtractionQueue, WhatsAppImageExtractionQueue>();

            services.AddHostedService<WhatsAppConversationWorker>();
            services.AddHostedService<WhatsAppImageExtractionWorker>();

            return services;
        }
    }
}
