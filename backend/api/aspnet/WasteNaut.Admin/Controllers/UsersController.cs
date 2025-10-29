using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;
using WasteNaut.Admin.Services;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly WasteNautDbContext _context;

        public UsersController(IUserService userService, WasteNautDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Organization)
                    .ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving users", error = ex.Message });
            }
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user", error = ex.Message });
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return Conflict(new { message = "User with this email already exists" });
                }

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role,
                    Status = "pending",
                    Phone = request.Phone,
                    Address = request.Address,
                    OrganizationId = request.OrganizationId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user", error = ex.Message });
            }
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Update user properties
                user.Name = request.Name ?? user.Name;
                user.Email = request.Email ?? user.Email;
                user.Role = request.Role ?? user.Role;
                user.Status = request.Status ?? user.Status;
                user.Phone = request.Phone ?? user.Phone;
                user.Address = request.Address ?? user.Address;
                user.OrganizationId = request.OrganizationId ?? user.OrganizationId;
                user.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "User updated successfully", user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user", error = ex.Message });
            }
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
            }
        }

        // POST: api/users/5/suspend
        [HttpPost("{id}/suspend")]
        public async Task<IActionResult> SuspendUser(int id, [FromBody] SuspendUserRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                user.Status = "suspended";
                user.SuspensionReason = request.Reason;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "User suspended successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error suspending user", error = ex.Message });
            }
        }

        // POST: api/users/5/activate
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                user.Status = "active";
                user.SuspensionReason = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error activating user", error = ex.Message });
            }
        }

        // GET: api/users/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetUserStats()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.Status == "active");
                var pendingUsers = await _context.Users.CountAsync(u => u.Status == "pending");
                var suspendedUsers = await _context.Users.CountAsync(u => u.Status == "suspended");

                var roleStats = await _context.Users
                    .GroupBy(u => u.Role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    totalUsers,
                    activeUsers,
                    pendingUsers,
                    suspendedUsers,
                    roleStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user statistics", error = ex.Message });
            }
        }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "individual";
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? OrganizationId { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? OrganizationId { get; set; }
    }
}