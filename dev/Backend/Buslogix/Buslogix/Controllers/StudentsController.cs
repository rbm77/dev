using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentsController(IStudentService studentService) : ControllerBase
    {

        private readonly IStudentService _studentService = studentService;

        [Authorize(Policy = $"{Resources.STUDENT}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetStudents(
            [FromQuery] bool? isActive = null,
            [FromQuery] string? identityDocument = null,
            [FromQuery] string? name = null,
            [FromQuery] string? lastName = null,
            [FromQuery] int? routeId = null,
            [FromQuery] int? gradeId = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<Student> students = await _studentService.GetStudents(companyId, isActive, identityDocument, name, lastName, routeId, gradeId);
            return Ok(students);
        }

        [Authorize(Policy = $"{Resources.STUDENT}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            Student? student = await _studentService.GetStudent(companyId, id);
            return student == null ? NotFound() : Ok(student);
        }

        [Authorize(Policy = $"{Resources.STUDENT}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertStudent([FromBody] Student student)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _studentService.InsertStudent(companyId, student);
            return id > 0 ? CreatedAtAction(nameof(GetStudent), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.STUDENT}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student student)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _studentService.UpdateStudent(companyId, id, student);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.STUDENT}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _studentService.DeleteStudent(companyId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}