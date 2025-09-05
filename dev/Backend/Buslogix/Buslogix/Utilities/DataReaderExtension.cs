using System.Data;

namespace Buslogix.Utilities
{
    public static class DataReaderExtension
    {

        public static int GetInt32OrDefault(this IDataRecord reader, int ordinal, int defaultValue = 0)
            => reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);

        public static string GetStringOrDefault(this IDataRecord reader, int ordinal, string defaultValue = "")
            => reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);

        public static bool GetBooleanOrDefault(this IDataRecord reader, int ordinal, bool defaultValue = false)
            => reader.IsDBNull(ordinal) ? defaultValue : reader.GetBoolean(ordinal);

        public static DateTime? GetDateTimeOrDefault(this IDataRecord reader, int ordinal, DateTime? defaultValue = null)
            => reader.IsDBNull(ordinal) ? defaultValue : reader.GetDateTime(ordinal);

        public static decimal GetDecimalOrDefault(this IDataReader reader, int ordinal, decimal defaultValue = 0m)
            => reader.IsDBNull(ordinal) ? defaultValue : reader.GetDecimal(ordinal);
    }
}
