using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Queue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction
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
