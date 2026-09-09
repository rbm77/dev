using Buslogix.Features.EmailIngestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.EmailIngestion
{
    [Route("email-ingestion")]
    [ApiController]
    public class EmailIngestionController(EmailPollQueue emailPollQueue) : ControllerBase
    {

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("poll")]
        public IActionResult Poll()
        {
            if (HttpContext.GetServiceName() != "EmailRetrievalService") return Forbid();
            emailPollQueue.TryTrigger();
            return Accepted();
        }
    }
}
