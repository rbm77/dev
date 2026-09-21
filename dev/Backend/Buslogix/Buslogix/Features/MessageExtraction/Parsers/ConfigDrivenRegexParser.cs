using System.Globalization;
using System.Text.RegularExpressions;
using Buslogix.Common.Llm;
using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Configuration;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction.Parsers
{
    internal class ConfigDrivenRegexParser(IPatternProvider patternProvider, ILogHandler logHandler) : IMessageParser
    {
        public bool TryParse(string message, out ExtractedData? data)
        {
            foreach (CompiledPattern pattern in patternProvider.GetPatterns())
            {
                Match match;
                try
                {
                    match = pattern.Regex.Match(message);
                }
                catch (RegexMatchTimeoutException)
                {
                    _ = logHandler.WriteLog($"Pattern {pattern.Name} timed out against message, skipping.", LogType.Warning);
                    continue;
                }

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
