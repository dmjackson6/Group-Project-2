using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly WasteNautDbContext _context;
        private readonly IAuditService _auditService;

        public OrganizationService(WasteNautDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Organization>> GetOrganizationsAsync(string? status, string? type)
        {
            var query = _context.Organizations.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(o => o.Type == type);

            return await query.ToListAsync();
        }

        public async Task<Organization?> GetByIdAsync(int id)
        {
            return await _context.Organizations.FindAsync(id);
        }

        public async Task ApproveOrganizationAsync(int orgId)
        {
            var org = await _context.Organizations.FindAsync(orgId);
            if (org == null)
                throw new ArgumentException("Organization not found");

            if (org.Status == "approved")
                throw new ArgumentException("Organization is already approved");

            org.Status = "approved";
            org.ApprovedAt = DateTime.UtcNow;
            org.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "organization_approved", "Organization", orgId, "Organization approved");
        }

        public async Task RejectOrganizationAsync(int orgId, string reason)
        {
            var org = await _context.Organizations.FindAsync(orgId);
            if (org == null)
                throw new ArgumentException("Organization not found");

            if (org.Status == "rejected")
                throw new ArgumentException("Organization is already rejected");

            org.Status = "rejected";
            org.RejectedAt = DateTime.UtcNow;
            org.RejectionReason = reason;
            org.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "organization_rejected", "Organization", orgId, $"Organization rejected: {reason}");
        }

        public async Task SetCapacityAsync(int orgId, int max, int used, string? notes)
        {
            var org = await _context.Organizations.FindAsync(orgId);
            if (org == null)
                throw new ArgumentException("Organization not found");

            org.CapacityMax = max;
            org.CapacityUsed = used;
            org.CapacityNotes = notes;
            org.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "organization_capacity_updated", "Organization", orgId, $"Capacity updated: {used}/{max}");
        }
    }
}
