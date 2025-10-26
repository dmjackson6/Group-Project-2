using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public class UserService : IUserService
    {
        private readonly WasteNautDbContext _context;
        private readonly IAuditService _auditService;

        public UserService(WasteNautDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int limit, string? status, string? role, string? search)
        {
            var query = _context.Users
                .Include(u => u.Organization)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));

            return await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task VerifyUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (user.Status == "verified")
                throw new ArgumentException("User is already verified");

            user.Status = "verified";
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "user_verified", "User", userId, "User account verified");
        }

        public async Task SuspendUserAsync(int userId, string reason)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (user.Status == "suspended")
                throw new ArgumentException("User is already suspended");

            user.Status = "suspended";
            user.SuspensionReason = reason;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "user_suspended", "User", userId, $"User suspended: {reason}");
        }

        public async Task UnsuspendUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (user.Status != "suspended")
                throw new ArgumentException("User is not suspended");

            user.Status = "verified";
            user.SuspensionReason = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "user_unsuspended", "User", userId, "User unsuspended");
        }

        public async Task<ImpersonationResponseDto> ImpersonateUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (user.Status != "verified")
                throw new ArgumentException("Cannot impersonate inactive user");

            // In real implementation, generate impersonation token
            var impersonationToken = $"impersonation-token-{userId}-{DateTime.UtcNow.Ticks}";

            await _auditService.LogActivityAsync(0, "user_impersonation_started", "User", userId, "Impersonation session started");

            return new ImpersonationResponseDto
            {
                ImpersonationToken = impersonationToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }
    }
}
