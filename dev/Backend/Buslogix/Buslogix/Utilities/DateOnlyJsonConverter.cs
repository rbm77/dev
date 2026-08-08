using System.Text.Json;
using System.Text.Json.Serialization;

namespace Buslogix.Utilities
{
    public class DateOnlyJsonConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.ParseExact(reader.GetString()!, Format, System.Globalization.CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format, System.Globalization.CultureInfo.InvariantCulture));
    }

    public class NullableDateOnlyJsonConverter : JsonConverter<DateTime?>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null
                ? null
                : DateTime.ParseExact(reader.GetString()!, Format, System.Globalization.CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString(Format, System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
