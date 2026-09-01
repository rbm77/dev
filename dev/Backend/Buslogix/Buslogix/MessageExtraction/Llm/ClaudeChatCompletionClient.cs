using System.Text.Json;
using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Messages;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.MessageExtraction.Configuration;
using Microsoft.Extensions.Options;
using static Buslogix.Utilities.Enums;

namespace Buslogix.MessageExtraction.Llm
{
    /// <summary>
    /// The only place in this feature that talks to the Anthropic API. Uses the
    /// official Anthropic SDK against a shared singleton AnthropicClient; the model
    /// id is configuration-driven (LlmFallbackOptions.Model) instead of hardcoded.
    /// </summary>
    public class ClaudeChatCompletionClient(
        AnthropicClient anthropicClient,
        IOptionsMonitor<LlmFallbackOptions> optionsMonitor,
        ILogHandler logHandler) : IChatCompletionClient
    {
        // Short, single-JSON-object extraction ({amount, reference, date}) never
        // needs a large output ceiling; keep this tight.
        private const int MaxTokens = 1024;

        // Mirrors ExtractedData(decimal Amount, string Reference, DateTime? Date).
        // Forcing this schema via structured outputs means the response is
        // guaranteed to parse - JsonHelpers.TryDeserialize failing becomes
        // effectively impossible instead of something we hope the prompt prevents.
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

        public async Task<string?> CompleteAsync(string prompt, CancellationToken ct)
        {
            string model = optionsMonitor.CurrentValue.Model;

            try
            {
                var response = await anthropicClient.Messages.Create(
                    new MessageCreateParams
                    {
                        Model = model,
                        MaxTokens = MaxTokens,
                        Messages = [new() { Role = Role.User, Content = prompt }],
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
                // The account's prepaid credit balance is exhausted (or a payment
                // method needs attention). Logged as Error, not Warning, so this
                // doesn't get lost among routine rate-limit/5xx retries - it needs a
                // human to recharge at console.anthropic.com/settings/billing.
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
