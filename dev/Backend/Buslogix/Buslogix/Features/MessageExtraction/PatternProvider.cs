using System.Text.RegularExpressions;
using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Configuration;
using Microsoft.Extensions.Options;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction
{
    internal class PatternProvider : IPatternProvider
    {
        private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(2);

        private readonly ILogHandler logHandler;
        private IReadOnlyList<CompiledPattern> patterns;

        public PatternProvider(IOptionsMonitor<ExtractionPatternsOptions> optionsMonitor, ILogHandler logHandler)
        {
            this.logHandler = logHandler;
            patterns = Compile(optionsMonitor.CurrentValue);
            optionsMonitor.OnChange(updated => patterns = Compile(updated));
        }

        public IReadOnlyList<CompiledPattern> GetPatterns() => patterns;

        private IReadOnlyList<CompiledPattern> Compile(ExtractionPatternsOptions options)
        {
            using IDisposable logScope = LogScope.Push(LogScope.NewId(), null);
            List<CompiledPattern> compiled = [];

            foreach (PatternDefinition definition in options.Patterns)
            {
                try
                {
                    Regex regex = new(definition.Regex, RegexOptions.Compiled | RegexOptions.Singleline, MatchTimeout);
                    compiled.Add(new CompiledPattern(definition.Name, regex, definition.AmountGroup, definition.ReferenceGroup, definition.DateGroup));
                }
                catch (ArgumentException ex)
                {
                    _ = logHandler.WriteLog($"Skipping malformed extraction pattern {definition.Name}: {ex.Message}", LogType.Warning);
                }
            }

            return compiled;
        }
    }
}
