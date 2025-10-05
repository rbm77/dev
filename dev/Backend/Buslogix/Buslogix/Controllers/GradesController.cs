using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GradesController(IGradeService gradeService) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetGrades([FromQuery] string? description = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Grade> items = await gradeService.GetGrades(companyId, description);
            return Ok(items);
        }

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGrade(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Grade? item = await gradeService.GetGrade(companyId, id);
            return item == null ? NotFound() : Ok(item);
        }

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertGrade([FromBody] Grade grade)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await gradeService.InsertGrade(companyId, grade);
            return id > 0 ? CreatedAtAction(nameof(GetGrade), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] Grade grade)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await gradeService.UpdateGrade(companyId, id, grade);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGrade(int id, [FromServices] IUserService userService)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await gradeService.DeleteGrade(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}