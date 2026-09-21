using Buslogix.Common.Llm;

namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageExtractionResultRepository
    {
        Task Insert(int companyId, ExtractedData data);
    }
}
