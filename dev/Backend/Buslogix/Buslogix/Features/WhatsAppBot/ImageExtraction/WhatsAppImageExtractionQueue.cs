using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.WhatsAppBot.Abstractions;

namespace Buslogix.Features.WhatsAppBot.ImageExtraction
{
    public class WhatsAppImageExtractionQueue : ItemQueue<WhatsAppImageExtractionItem>, IWhatsAppImageExtractionQueue;
}
