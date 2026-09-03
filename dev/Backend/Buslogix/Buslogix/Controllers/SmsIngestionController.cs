using Buslogix.MessageExtraction.Abstractions;
using Buslogix.MessageExtraction.Queue;
using Buslogix.Models.DTO;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Controllers
{
    [Route("sms-ingestion")]
    [ApiController]
    public class SmsIngestionController(IMessageIngestionQueue messageIngestionQueue) : ControllerBase
    {

        [Authorize(AuthenticationSchemes = ServiceAuth.SchemeName)]
        [HttpPost("receive")]
        public async Task<IActionResult> Receive([FromBody] SmsMessageDto dto)
        {
            if (HttpContext.GetServiceName() != "SmsIngestionService") return Forbid();
            if (string.IsNullOrWhiteSpace(dto.MessageText)) return BadRequest();

            await messageIngestionQueue.EnqueueAsync(
                new IngestionItem(IngestionSource.Sms, dto.CompanyId, dto.MessageText, DateTime.UtcNow));

            return Accepted();
        }
    }
}
