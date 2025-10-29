using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Services;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users with optional filtering
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="limit">Items per page</param>
        /// <param name="status">Filter by status</param>
        /// <param name="role">Filter by role</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? role = null,
            [FromQuery] string? search = null)
        {
            try
            {
                var users = await _userService.GetUsersAsync(page, limit, status, role, search);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify user account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{userId}/verify")]
        public async Task<IActionResult> VerifyUser(int userId)
        {
            try
            {
                await _userService.VerifyUserAsync(userId);
                return Ok(new { message = "User verified successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Suspend user account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Suspension details</param>
        /// <returns>Success message</returns>
        [HttpPost("{userId}/suspend")]
        public async Task<IActionResult> SuspendUser(int userId, [FromBody] SuspendUserRequestDto request)
        {
            try
            {
                await _userService.SuspendUserAsync(userId, request.Reason);
                return Ok(new { message = "User suspended successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Unsuspend user account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{userId}/unsuspend")]
        public async Task<IActionResult> UnsuspendUser(int userId)
        {
            try
            {
                await _userService.UnsuspendUserAsync(userId);
                return Ok(new { message = "User unsuspended successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Start impersonation session
        /// </summary>
        /// <param name="userId">User ID to impersonate</param>
        /// <returns>Impersonation token</returns>
        [HttpPost("{userId}/impersonate")]
        public async Task<IActionResult> ImpersonateUser(int userId)
        {
            try
            {
                var result = await _userService.ImpersonateUserAsync(userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
