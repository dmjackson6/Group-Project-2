using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public class AdminService : IAdminService
    {
        private readonly WasteNautDbContext _context;
        private readonly IAuditService _auditService;

        public AdminService(WasteNautDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var admin = await GetByEmailAsync(request.Email);
            if (admin == null || !await ValidatePasswordAsync(admin, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (admin.Status != "active")
            {
                throw new UnauthorizedAccessException("Account is not active");
            }

            // Update last login
            admin.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Log login activity
            await _auditService.LogActivityAsync(admin.Id, "admin_login", "Admin", admin.Id, "Admin logged in");

            // Generate JWT token (in real implementation)
            var token = GenerateJwtToken(admin);

            return new LoginResponseDto
            {
                Success = true,
                Token = token,
                User = new AdminDto
                {
                    Id = admin.Id,
                    Name = admin.Name,
                    Email = admin.Email,
                    Role = admin.Role.Name,
                    Status = admin.Status
                }
            };
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _context.Admins
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _context.Admins
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> ValidatePasswordAsync(Admin admin, string password)
        {
            // In real implementation, use proper password hashing
            // For now, this is a placeholder
            return await Task.FromResult(true);
        }

        public async Task LogActivityAsync(int adminId, string action, string target, int? targetId, string details)
        {
            await _auditService.LogActivityAsync(adminId, action, target, targetId, details);
        }

        private string GenerateJwtToken(Admin admin)
        {
            // In real implementation, generate actual JWT token
            // For now, return a mock token
            return $"mock-jwt-token-{admin.Id}-{DateTime.UtcNow.Ticks}";
        }
    }
}
