using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.MessageExtraction.Abstractions;

namespace Buslogix.Features.MessageExtraction.Ingestion
{
    public class MessageIngestionQueue : ItemQueue<IngestionItem>, IMessageIngestionQueue;
}
