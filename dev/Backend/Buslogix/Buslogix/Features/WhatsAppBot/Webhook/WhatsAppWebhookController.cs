using System.Text.Json;
using Buslogix.Features.WhatsAppBot.Abstractions;
using Buslogix.Features.WhatsAppBot.Configuration;
using Buslogix.Features.WhatsAppBot.Conversation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.WhatsAppBot.Webhook
{
    [AllowAnonymous]
    [Route("whatsapp-webhook")]
    [ApiController]
    public class WhatsAppWebhookController(
        IWhatsAppSignatureValidator signatureValidator,
        IWhatsAppConversationQueue conversationQueue,
        IOptionsMonitor<WhatsAppOptions> optionsMonitor,
        ILogHandler logHandler) : ControllerBase
    {
        [HttpGet]
        public IActionResult VerifyWebhook(
            [FromQuery(Name = "hub.mode")] string? mode,
            [FromQuery(Name = "hub.verify_token")] string? verifyToken,
            [FromQuery(Name = "hub.challenge")] string? challenge)
        {
            if (mode == "subscribe" &&
                !string.IsNullOrEmpty(challenge) &&
                verifyToken == optionsMonitor.CurrentValue.VerifyToken)
            {
                return Content(challenge, "text/plain");
            }

            return Forbid();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveWebhook()
        {
            using MemoryStream buffer = new();
            await Request.Body.CopyToAsync(buffer);
            byte[] rawBody = buffer.ToArray();

            string? signatureHeader = Request.Headers.TryGetValue("X-Hub-Signature-256", out var values) ? values.FirstOrDefault() : null;
            if (!signatureValidator.IsValid(rawBody, signatureHeader))
            {
                await logHandler.WriteLog("WhatsApp webhook: rejected request with an invalid or missing signature.", LogType.Warning);
                return Unauthorized();
            }

            WhatsAppWebhookPayload? payload;
            try
            {
                payload = JsonSerializer.Deserialize<WhatsAppWebhookPayload>(rawBody);
            }
            catch (JsonException ex)
            {
                await logHandler.WriteLog($"WhatsApp webhook: could not parse payload: {ex.Message}", LogType.Warning);
                return Ok();
            }

            foreach (WhatsAppWebhookEntry entry in payload?.Entry ?? [])
            {
                foreach (WhatsAppWebhookChange change in entry.Changes ?? [])
                {
                    string? phoneNumberId = change.Value?.Metadata?.PhoneNumberId;
                    if (string.IsNullOrEmpty(phoneNumberId))
                    {
                        continue;
                    }

                    foreach (WhatsAppWebhookMessage message in change.Value?.Messages ?? [])
                    {
                        WhatsAppInboundMessage? inbound = MapMessage(phoneNumberId, message);
                        if (inbound is not null)
                        {
                            await conversationQueue.Enqueue(inbound);
                        }
                    }
                }
            }

            return Ok();
        }

        private static WhatsAppInboundMessage? MapMessage(string phoneNumberId, WhatsAppWebhookMessage message)
        {
            if (string.IsNullOrEmpty(message.From))
            {
                return null;
            }

            string? interactiveReplyId = message.Interactive?.ButtonReply?.Id ?? message.Interactive?.ListReply?.Id;

            return message.Type switch
            {
                "text" when message.Text?.Body is not null =>
                    new WhatsAppInboundMessage(phoneNumberId, message.From, WhatsAppMessageType.Text, message.Text.Body, null, null, DateTime.UtcNow),
                "image" when message.Image?.Id is not null =>
                    new WhatsAppInboundMessage(phoneNumberId, message.From, WhatsAppMessageType.Image, null, message.Image.Id, null, DateTime.UtcNow),
                "interactive" when interactiveReplyId is not null =>
                    new WhatsAppInboundMessage(phoneNumberId, message.From, WhatsAppMessageType.Interactive, null, null, interactiveReplyId, DateTime.UtcNow),
                _ => null
            };
        }
    }
}
