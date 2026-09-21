using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.WhatsAppBot.ImageExtraction;

namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppImageExtractionQueue : IItemQueue<WhatsAppImageExtractionItem>;
}
