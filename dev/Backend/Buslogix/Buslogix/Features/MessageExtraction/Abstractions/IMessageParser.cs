using Buslogix.Common.Llm;

namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageParser
    {
        bool TryParse(string message, out ExtractedData? data);
    }
}
