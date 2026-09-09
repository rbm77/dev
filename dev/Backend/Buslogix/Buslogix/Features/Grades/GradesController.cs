using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.Grades
{
    [Route("[controller]")]
    [ApiController]
    public class GradesController(IGradeRepository gradeRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetGrades([FromQuery] string? description = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            int companyId = HttpContext.GetCompanyId();
            PagedResult<Grade> items = await gradeRepository.GetGrades(companyId, description, page, pageSize);
            return Ok(items);
        }

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGrade(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Grade? item = await gradeRepository.GetGrade(companyId, id);
            return item == null ? NotFound() : Ok(item);
        }

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertGrade([FromBody] Grade grade)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await gradeRepository.InsertGrade(companyId, grade);
            return id > 0 ? CreatedAtAction(nameof(GetGrade), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.GRADE}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] Grade grade)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = (await gradeRepository.UpdateGrade(companyId, id, grade)) > 0;
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
            bool deleted = (await gradeRepository.DeleteGrade(companyId, id)) > 0;
            return deleted ? NoContent() : NotFound();
        }
    }
}