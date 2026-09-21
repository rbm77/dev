using System.Text.Json.Serialization;

namespace Buslogix.Features.WhatsAppBot.Graph
{
    internal class WhatsAppMediaMetadata
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }
    }
}
