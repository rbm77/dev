using Buslogix.Interfaces;
using Buslogix.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("qr-codes")]
    [ApiController]
    public class QrCodesController(IQrCodeService qrCodeService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public IActionResult GenerateQrCodes([FromBody] QrCodeGenerationRequest request)
        {
            List<QrCodeResponseItem> result = qrCodeService.GenerateQrCodes(request.Items, request.Size);
            return Ok(result);
        }
    }
}
