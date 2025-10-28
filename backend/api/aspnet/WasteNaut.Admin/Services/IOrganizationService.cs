using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetOrganizationsAsync(string? status, string? type);
        Task<Organization?> GetByIdAsync(int id);
        Task ApproveOrganizationAsync(int orgId);
        Task RejectOrganizationAsync(int orgId, string reason);
        Task SetCapacityAsync(int orgId, int max, int used, string? notes);
    }
}
