using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public interface IAuditService
    {
        Task LogActivityAsync(int adminId, string action, string target, int? targetId, string details);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int limit = 50, string? action = null);
        Task<AuditLog?> GetAuditLogAsync(int logId);
    }
}
