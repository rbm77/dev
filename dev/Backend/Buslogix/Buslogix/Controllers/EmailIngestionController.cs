using Buslogix.EmailIngestion.Abstractions;
using Buslogix.Models.DTO;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("email-ingestion")]
    [ApiController]
    public class EmailIngestionController(IEmailIngestionService emailIngestionService) : ControllerBase
    {

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("poll")]
        public async Task<IActionResult> Poll()
        {
            if (HttpContext.GetServiceName() != "EmailRetrievalService") return Forbid();
            EmailIngestionResult result = await emailIngestionService.ProcessAllAccountsAsync();
            return Ok(result);
        }
    }
}
