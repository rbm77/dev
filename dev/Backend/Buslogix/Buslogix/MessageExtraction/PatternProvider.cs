using System.Text.RegularExpressions;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.MessageExtraction.Configuration;
using Microsoft.Extensions.Options;
using static Buslogix.Utilities.Enums;

namespace Buslogix.MessageExtraction
{
    /// <summary>
    /// Compiles the regex patterns from configuration once, and recompiles/atomically
    /// swaps them whenever the backing configuration file changes on disk. Regular
    /// message processing (GetPatterns) never touches the file system or recompiles
    /// anything - it just reads the current immutable list reference, lock-free.
    /// </summary>
    internal class PatternProvider : IPatternProvider
    {
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
            List<CompiledPattern> compiled = [];

            foreach (PatternDefinition definition in options.Patterns)
            {
                try
                {
                    Regex regex = new(definition.Regex, RegexOptions.Compiled | RegexOptions.Singleline);
                    compiled.Add(new CompiledPattern(definition.Name, regex, definition.AmountGroup, definition.ReferenceGroup, definition.DateGroup));
                }
                catch (ArgumentException ex)
                {
                    // Compile() runs synchronously from the constructor and from the
                    // (also synchronous) OptionsMonitor.OnChange callback, so the async
                    // WriteLog call is intentionally fire-and-forget here.
                    _ = logHandler.WriteLog($"Skipping malformed extraction pattern {definition.Name}: {ex.Message}", LogType.Warning);
                }
            }

            return compiled;
        }
    }
}
