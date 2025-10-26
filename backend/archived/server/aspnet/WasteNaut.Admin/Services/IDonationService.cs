using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public interface IDonationService
    {
        Task<IEnumerable<Donation>> GetDonationsAsync(string? status);
        Task<Donation?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int donationId, string status);
    }
}
