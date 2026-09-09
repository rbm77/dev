using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.MessageExtraction
{
    [Route("message-extraction")]
    [ApiController]
    public class MessageExtractionController(
        MessageExtractionRetryQueue messageExtractionRetryQueue,
        MessageExtractionPurgeQueue messageExtractionPurgeQueue) : ControllerBase
    {
        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("retry-failures")]
        public IActionResult RetryFailures()
        {
            if (HttpContext.GetServiceName() != "MessageExtractionRetryService") return Forbid();
            messageExtractionRetryQueue.TryTrigger();
            return Accepted();
        }

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("purge-history")]
        public IActionResult PurgeHistory()
        {
            if (HttpContext.GetServiceName() != "MessageExtractionPurgeService") return Forbid();
            messageExtractionPurgeQueue.TryTrigger();
            return Accepted();
        }
    }
}
