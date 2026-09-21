using System.Text.Json.Serialization;

namespace Buslogix.Features.WhatsAppBot.Webhook
{
    public class WhatsAppWebhookPayload
    {
        [JsonPropertyName("entry")]
        public List<WhatsAppWebhookEntry>? Entry { get; set; }
    }

    public class WhatsAppWebhookEntry
    {
        [JsonPropertyName("changes")]
        public List<WhatsAppWebhookChange>? Changes { get; set; }
    }

    public class WhatsAppWebhookChange
    {
        [JsonPropertyName("value")]
        public WhatsAppWebhookValue? Value { get; set; }
    }

    public class WhatsAppWebhookValue
    {
        [JsonPropertyName("metadata")]
        public WhatsAppWebhookMetadata? Metadata { get; set; }

        [JsonPropertyName("messages")]
        public List<WhatsAppWebhookMessage>? Messages { get; set; }
    }

    public class WhatsAppWebhookMetadata
    {
        [JsonPropertyName("phone_number_id")]
        public string? PhoneNumberId { get; set; }
    }

    public class WhatsAppWebhookMessage
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public WhatsAppWebhookText? Text { get; set; }

        [JsonPropertyName("image")]
        public WhatsAppWebhookImage? Image { get; set; }

        [JsonPropertyName("interactive")]
        public WhatsAppWebhookInteractive? Interactive { get; set; }
    }

    public class WhatsAppWebhookInteractive
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("button_reply")]
        public WhatsAppWebhookReply? ButtonReply { get; set; }

        [JsonPropertyName("list_reply")]
        public WhatsAppWebhookReply? ListReply { get; set; }
    }

    public class WhatsAppWebhookReply
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }

    public class WhatsAppWebhookText
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    public class WhatsAppWebhookImage
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }
    }
}
