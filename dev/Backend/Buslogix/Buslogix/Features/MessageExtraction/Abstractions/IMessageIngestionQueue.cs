using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.MessageExtraction.Ingestion;

namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageIngestionQueue : IItemQueue<IngestionItem>;
}
