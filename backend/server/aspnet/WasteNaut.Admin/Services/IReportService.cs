using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> GetReportsAsync(string? status, string? priority, string? type);
        Task<Report?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int reportId, string status, string? notes);
        Task AddNoteAsync(int reportId, string text, string type);
        Task ResolveReportAsync(int reportId, string resolution);
        Task AssignReportsAsync(int[] reportIds, int adminId);
    }
}
