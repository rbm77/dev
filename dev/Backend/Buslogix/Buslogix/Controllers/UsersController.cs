using System.Security.Claims;
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
    public class UsersController(IUserService userService, ITokenHandler tokenHandler) : ControllerBase

    {
        private readonly IUserService _userService = userService;
        private readonly ITokenHandler _tokenHandler = tokenHandler;

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] Credentials credentials)
        {
            UserIdentity? userIdentity = await _userService.Authenticate(credentials);
            if (userIdentity != null && userIdentity.IsAuthenticated)
            {
                Token? token = _tokenHandler.GenerateJwtToken(userIdentity);
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
            await _userService.ResetPassword(credentials);
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
            List<User> users = await _userService.GetUsers(
                companyId, roleId, isActive, identityDocument, name, lastName);
            return Ok(users);
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            User? user = await _userService.GetUser(companyId, id);
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.WRITE}")]
        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] User user)
        {
            int companyId = HttpContext.GetCompanyId();
            int id = await _userService.InsertUser(companyId, user);
            return id > 0 ? CreatedAtAction(nameof(GetUser), new { id }, null) : BadRequest();
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            int companyId = HttpContext.GetCompanyId();
            bool updated = await _userService.UpdateUser(companyId, id, user);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.USER}.{PermissionMode.WRITE}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int companyId = HttpContext.GetCompanyId();
            bool deleted = await _userService.DeleteUser(companyId, id);
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.OWN_USER}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] Credentials credentials)
        {
            if (id != GetUserId()) return Forbid();
            if (string.IsNullOrEmpty(credentials.Password)) return BadRequest();

            int companyId = HttpContext.GetCompanyId();
            bool updated = await _userService.UpdatePassword(companyId, id, credentials.Password);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.OWN_USER}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}/own")]
        public async Task<IActionResult> UpdateOwnUser(int id, [FromBody] User user)
        {
            if (id != GetUserId())
            {
                return Forbid();
            }

            int companyId = HttpContext.GetCompanyId();
            bool updated = await _userService.UpdateOwnUser(companyId, id, user);
            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = $"{Resources.OWN_USER}.{PermissionMode.READ}")]
        [HttpGet("{id:int}/own")]
        public async Task<IActionResult> GetOwnUser(int id)
        {
            if (id != GetUserId())
            {
                return Forbid();
            }

            int companyId = HttpContext.GetCompanyId();
            User? user = await _userService.GetUser(companyId, id);
            return user == null ? NotFound() : Ok(user);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
