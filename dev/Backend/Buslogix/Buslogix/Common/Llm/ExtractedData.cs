using System.Text.RegularExpressions;

namespace Buslogix.Common.Llm
{
    public partial record ExtractedData(decimal Amount, string? Reference, DateTime? Date)
    {
        public static string? NormalizeReference(string? reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return null;
            }

            string trimmed = reference.Trim();
            return ReferenceFormat().IsMatch(trimmed) ? trimmed : null;
        }

        [GeneratedRegex("^[A-Za-z0-9]{1,50}$")]
        private static partial Regex ReferenceFormat();
    }
}
