using System.Text.Json;
using System.Text.RegularExpressions;

namespace Buslogix.MessageExtraction.Llm
{
    /// <summary>
    /// Defensive JSON parsing helper for LLM responses, which sometimes wrap their
    /// JSON payload in a markdown code fence.
    /// </summary>
    internal static class JsonHelpers
    {
        private static readonly Regex CodeFence = new(@"^```(?:json)?\s*|\s*```$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static bool TryDeserialize<T>(string? json, out T? result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            string cleaned = CodeFence.Replace(json.Trim(), string.Empty).Trim();

            try
            {
                result = JsonSerializer.Deserialize<T>(cleaned, SerializerOptions);
                return result is not null;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
