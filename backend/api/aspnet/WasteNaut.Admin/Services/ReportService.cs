using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public class ReportService : IReportService
    {
        private readonly WasteNautDbContext _context;
        private readonly IAuditService _auditService;

        public ReportService(WasteNautDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Report>> GetReportsAsync(string? status, string? priority, string? type)
        {
            var query = _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .Include(r => r.AssignedAdmin)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(r => r.Priority == priority);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(r => r.Type == type);

            return await query.ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .Include(r => r.AssignedAdmin)
                .Include(r => r.Notes)
                .Include(r => r.Evidence)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateStatusAsync(int reportId, string status, string? notes)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                throw new ArgumentException("Report not found");

            report.Status = status;
            report.UpdatedAt = DateTime.UtcNow;

            if (status == "resolved")
                report.ResolvedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(notes))
            {
                var note = new ReportNote
                {
                    ReportId = reportId,
                    Author = "Admin User", // In real implementation, get from current user
                    Text = notes,
                    Type = "internal"
                };
                _context.ReportNotes.Add(note);
            }

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "report_status_updated", "Report", reportId, $"Status updated to {status}");
        }

        public async Task AddNoteAsync(int reportId, string text, string type)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                throw new ArgumentException("Report not found");

            var note = new ReportNote
            {
                ReportId = reportId,
                Author = "Admin User", // In real implementation, get from current user
                Text = text,
                Type = type
            };

            _context.ReportNotes.Add(note);
            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "report_note_added", "Report", reportId, "Note added");
        }

        public async Task ResolveReportAsync(int reportId, string resolution)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                throw new ArgumentException("Report not found");

            report.Status = "resolved";
            report.ResolutionNotes = resolution;
            report.ResolvedAt = DateTime.UtcNow;
            report.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "report_resolved", "Report", reportId, $"Report resolved: {resolution}");
        }

        public async Task AssignReportsAsync(int[] reportIds, int adminId)
        {
            var reports = await _context.Reports
                .Where(r => reportIds.Contains(r.Id))
                .ToListAsync();

            foreach (var report in reports)
            {
                report.AssignedAdminId = adminId;
                report.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(adminId, "reports_assigned", "Reports", 0, $"Assigned {reportIds.Length} reports");
        }
    }
}
