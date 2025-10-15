using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public class AuditService : IAuditService
    {
        private readonly WasteNautDbContext _context;

        public AuditService(WasteNautDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(int adminId, string action, string target, int? targetId, string details)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                User = "Admin User", // In real implementation, get from current user
                UserId = adminId,
                Target = target,
                TargetId = targetId,
                Details = details,
                IpAddress = "127.0.0.1", // In real implementation, get from request
                UserAgent = "WasteNaut Admin", // In real implementation, get from request
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int limit = 50, string? action = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<AuditLog?> GetAuditLogAsync(int logId)
        {
            return await _context.AuditLogs.FindAsync(logId);
        }
    }
}
