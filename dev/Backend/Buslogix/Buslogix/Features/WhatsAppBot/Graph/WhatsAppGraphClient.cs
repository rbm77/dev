using System.Net.Http.Headers;
using System.Net.Http.Json;
using Buslogix.Features.WhatsAppBot.Abstractions;

namespace Buslogix.Features.WhatsAppBot.Graph
{
    public class WhatsAppGraphClient(HttpClient httpClient) : IWhatsAppGraphClient
    {
        private const long MaxMediaBytes = 10 * 1024 * 1024;

        public async Task SendTextMessage(string accessToken, string phoneNumberId, string toWaId, string text, CancellationToken ct)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, $"{phoneNumberId}/messages")
            {
                Content = JsonContent.Create(new
                {
                    messaging_product = "whatsapp",
                    to = toWaId,
                    type = "text",
                    text = new { body = text }
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendButtonsMessage(string accessToken, string phoneNumberId, string toWaId, string bodyText, IReadOnlyList<(string Id, string Title)> buttons, CancellationToken ct)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, $"{phoneNumberId}/messages")
            {
                Content = JsonContent.Create(new
                {
                    messaging_product = "whatsapp",
                    to = toWaId,
                    type = "interactive",
                    interactive = new
                    {
                        type = "button",
                        body = new { text = bodyText },
                        action = new
                        {
                            buttons = buttons.Select(button => new
                            {
                                type = "reply",
                                reply = new { id = button.Id, title = button.Title }
                            })
                        }
                    }
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendListMessage(string accessToken, string phoneNumberId, string toWaId, string bodyText, string menuLabel, IReadOnlyList<(string Id, string Title, string? Description)> rows, CancellationToken ct)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, $"{phoneNumberId}/messages")
            {
                Content = JsonContent.Create(new
                {
                    messaging_product = "whatsapp",
                    to = toWaId,
                    type = "interactive",
                    interactive = new
                    {
                        type = "list",
                        body = new { text = bodyText },
                        action = new
                        {
                            button = menuLabel,
                            sections = new[]
                            {
                                new
                                {
                                    rows = rows.Select(row => new { id = row.Id, title = row.Title, description = row.Description })
                                }
                            }
                        }
                    }
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task<(byte[] Bytes, string MimeType)> DownloadMedia(string accessToken, string mediaId, CancellationToken ct)
        {
            using HttpRequestMessage metadataRequest = new(HttpMethod.Get, mediaId);
            metadataRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using HttpResponseMessage metadataResponse = await httpClient.SendAsync(metadataRequest, ct);
            metadataResponse.EnsureSuccessStatusCode();

            WhatsAppMediaMetadata? metadata = await metadataResponse.Content.ReadFromJsonAsync<WhatsAppMediaMetadata>(ct);
            if (string.IsNullOrEmpty(metadata?.Url))
            {
                throw new InvalidOperationException($"WhatsApp media {mediaId} has no download URL.");
            }

            using HttpRequestMessage downloadRequest = new(HttpMethod.Get, metadata.Url);
            downloadRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using HttpResponseMessage downloadResponse = await httpClient.SendAsync(downloadRequest, ct);
            downloadResponse.EnsureSuccessStatusCode();

            long? contentLength = downloadResponse.Content.Headers.ContentLength;
            if (contentLength > MaxMediaBytes)
            {
                throw new InvalidOperationException($"WhatsApp media {mediaId} is {contentLength} bytes, which exceeds the {MaxMediaBytes} byte limit.");
            }

            byte[] bytes = await downloadResponse.Content.ReadAsByteArrayAsync(ct);
            if (bytes.LongLength > MaxMediaBytes)
            {
                throw new InvalidOperationException($"WhatsApp media {mediaId} is {bytes.LongLength} bytes, which exceeds the {MaxMediaBytes} byte limit.");
            }
            string mimeType = metadata.MimeType ?? downloadResponse.Content.Headers.ContentType?.MediaType ?? "image/jpeg";

            return (bytes, mimeType);
        }
    }
}
