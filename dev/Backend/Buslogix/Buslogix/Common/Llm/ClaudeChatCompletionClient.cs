using System.Text.Json;
using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Messages;
using Microsoft.Extensions.Options;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Common.Llm
{
    public class ClaudeChatCompletionClient(
        AnthropicClient anthropicClient,
        IOptionsMonitor<ClaudeOptions> optionsMonitor,
        ILogHandler logHandler) : IChatCompletionClient
    {
        private const int MaxTokens = 1024;

        private static readonly Dictionary<string, JsonElement> ExtractedDataSchema = new()
        {
            ["type"] = JsonSerializer.SerializeToElement("object"),
            ["properties"] = JsonSerializer.SerializeToElement(new
            {
                amount = new { type = "number" },
                reference = new { type = "string" },
                date = new
                {
                    type = new[] { "string", "null" },
                    description = "yyyy-MM-dd, or null if no date is present in the message"
                }
            }),
            ["required"] = JsonSerializer.SerializeToElement(new[] { "amount", "reference", "date" }),
            ["additionalProperties"] = JsonSerializer.SerializeToElement(false)
        };

        public Task<string?> Complete(string prompt, CancellationToken ct)
        {
            MessageParam message = new() { Role = Anthropic.Models.Messages.Role.User, Content = prompt };
            return CompleteInternal(message, ct);
        }

        public Task<string?> CompleteWithImage(byte[] imageBytes, string mediaType, string prompt, CancellationToken ct)
        {
            Base64ImageSource imageSource = new()
            {
                Data = Convert.ToBase64String(imageBytes),
                MediaType = mediaType
            };

            List<ContentBlockParam> content =
            [
                new ContentBlockParam(new ImageBlockParam(new ImageBlockParamSource(imageSource))),
                new ContentBlockParam(new TextBlockParam(prompt))
            ];

            MessageParam message = new() { Role = Anthropic.Models.Messages.Role.User, Content = content };
            return CompleteInternal(message, ct);
        }

        private async Task<string?> CompleteInternal(MessageParam message, CancellationToken ct)
        {
            string model = optionsMonitor.CurrentValue.Model;

            try
            {
                var response = await anthropicClient.Messages.Create(
                    new MessageCreateParams
                    {
                        Model = model,
                        MaxTokens = MaxTokens,
                        Messages = [message],
                        OutputConfig = new OutputConfig
                        {
                            Format = new JsonOutputFormat { Schema = ExtractedDataSchema }
                        }
                    },
                    cancellationToken: ct);

                return response.Content
                    .Select(b => b.Value)
                    .OfType<TextBlock>()
                    .FirstOrDefault()?.Text;
            }
            catch (AnthropicRateLimitException ex)
            {
                await logHandler.WriteLog($"Claude chat completion rate limited: {ex.Message}", LogType.Warning);
                return null;
            }
            catch (Anthropic5xxException ex)
            {
                await logHandler.WriteLog($"Claude chat completion failed with a server error: {ex.Message}", LogType.Warning);
                return null;
            }
            catch (AnthropicForbiddenException ex) when (ex.ErrorType == Anthropic.Models.ErrorType.BillingError)
            {
                await logHandler.WriteLog(
                    $"Claude chat completion blocked: billing_error (likely out of API credit - recharge at console.anthropic.com/settings/billing). Details: {ex.Message}",
                    LogType.Error);
                return null;
            }
            catch (AnthropicApiException ex)
            {
                await logHandler.WriteLog($"Claude chat completion API call failed: {ex.Message}", LogType.Warning);
                return null;
            }
        }
    }
}
