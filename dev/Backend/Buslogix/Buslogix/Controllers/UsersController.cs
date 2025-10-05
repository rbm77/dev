using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] Credentials credentials, [FromServices] ITokenHandler tokenHandler)
        {
            UserIdentity? userIdentity = await userService.Authenticate(credentials);
            if (userIdentity != null && userIdentity.IsAuthenticated)
            {
                Token? token = tokenHandler.GenerateToken(userIdentity);
                if (token != null)
                {
                    return Ok(token);
                }
            }
            return Unauthorized(new Error { Message = "Invalid credentials." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] Credentials credentials)
        {
            await userService.ResetPassword(credentials);
            return Accepted();
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int? roleId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? identityDocument = null,
            [FromQuery] string? name = null,
            [FromQuery] string? lastName = null)
        {
            int companyId = HttpContext.GetCompanyId();
            List<User> users = await userService.GetUsers(
                companyId, roleId, isActive, identityDocument, name, lastName);
            return Ok(users);
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            User? user = await userService.GetUser(companyId, id);
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] User user)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await userService.InsertUser(companyId, user);
            return id > 0 ? CreatedAtAction(nameof(GetUser), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool updated = await userService.UpdateUser(companyId, id, user);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            if (!await userService.IsCriticalProcessUser(companyId, HttpContext.GetUserId()))
            {
                return Forbid();
            }
            bool deleted = await userService.DeleteUser(companyId, id);
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.OWN_USER}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] Credentials credentials)
        {
            if (id != HttpContext.GetUserId()) return Forbid();
            if (string.IsNullOrEmpty(credentials.Password)) return BadRequest();

            int companyId = HttpContext.GetCompanyId();
            bool updated = await userService.UpdatePassword(companyId, id, credentials.Password);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.OWN_USER}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/own")]
        public async Task<IActionResult> UpdateOwnUser(int id, [FromBody] User user)
        {
            if (id != HttpContext.GetUserId())
            {
                return Forbid();
            }

            int companyId = HttpContext.GetCompanyId();
            bool updated = await userService.UpdateOwnUser(companyId, id, user);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.OWN_USER}.{PermissionMode.READ}")]
        [HttpGet("{id:int}/own")]
        public async Task<IActionResult> GetOwnUser(int id)
        {
            if (id != HttpContext.GetUserId())
            {
                return Forbid();
            }

            int companyId = HttpContext.GetCompanyId();
            User? user = await userService.GetUser(companyId, id);
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.READ}")]
        [HttpGet("critical-process")]
        public async Task<IActionResult> GetCriticalProcessUsers()
        {
            int companyId = HttpContext.GetCompanyId();
            List<CriticalProcessUser> users = await userService.GetCriticalProcessUsers(companyId);
            return Ok(users);
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.WRITE}")]
        [HttpPut("critical-process")]
        public async Task<IActionResult> UpdateCriticalProcessUsers([FromBody] List<CriticalProcessUser> users)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await userService.UpdateCriticalProcessUsers(companyId, users);
            return updated ? NoContent() : NotFound();
        }
    }
}
