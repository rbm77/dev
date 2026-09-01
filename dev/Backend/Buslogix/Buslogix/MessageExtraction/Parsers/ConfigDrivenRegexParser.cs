using System.Globalization;
using System.Text.RegularExpressions;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.MessageExtraction.Configuration;

namespace Buslogix.MessageExtraction.Parsers
{
    /// <summary>
    /// Tier 1 parser. Does not hardcode any pattern - iterates the compiled patterns
    /// supplied by IPatternProvider, in order, and returns on the first one whose
    /// named capture groups successfully map to ExtractedData.
    /// </summary>
    internal class ConfigDrivenRegexParser(IPatternProvider patternProvider) : IMessageParser
    {
        public bool TryParse(string message, out ExtractedData? data)
        {
            foreach (CompiledPattern pattern in patternProvider.GetPatterns())
            {
                Match match = pattern.Regex.Match(message);
                if (!match.Success)
                {
                    continue;
                }

                Group amountGroup = match.Groups[pattern.AmountGroup];
                Group referenceGroup = match.Groups[pattern.ReferenceGroup];
                if (!amountGroup.Success || !referenceGroup.Success)
                {
                    continue;
                }

                if (!decimal.TryParse(amountGroup.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
                {
                    continue;
                }

                DateTime? date = null;
                if (pattern.DateGroup is not null)
                {
                    Group dateGroup = match.Groups[pattern.DateGroup];
                    if (dateGroup.Success && DateTime.TryParseExact(dateGroup.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        date = parsedDate;
                    }
                }

                data = new ExtractedData(amount, referenceGroup.Value, date);
                return true;
            }

            data = null;
            return false;
        }
    }
}
