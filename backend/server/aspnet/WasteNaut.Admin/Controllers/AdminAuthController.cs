using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Services;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminAuthController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Admin login endpoint
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token and admin information</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _adminService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current admin profile
        /// </summary>
        /// <returns>Current admin information</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var adminId = GetCurrentAdminId();
                var admin = await _adminService.GetByIdAsync(adminId);
                
                if (admin == null)
                    return NotFound(new { message = "Admin not found" });

                return Ok(new
                {
                    id = admin.Id,
                    name = admin.Name,
                    email = admin.Email,
                    role = admin.Role.Name,
                    status = admin.Status,
                    lastLogin = admin.LastLogin
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Logout current admin
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // In a real implementation, you might want to blacklist the JWT token
            return Ok(new { message = "Logged out successfully" });
        }

        private int GetCurrentAdminId()
        {
            var adminIdClaim = User.FindFirst("sub")?.Value;
            if (int.TryParse(adminIdClaim, out int adminId))
                return adminId;
            
            throw new UnauthorizedAccessException("Invalid admin ID in token");
        }
    }
}
