using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public class DonationService : IDonationService
    {
        private readonly WasteNautDbContext _context;
        private readonly IAuditService _auditService;

        public DonationService(WasteNautDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Donation>> GetDonationsAsync(string? status)
        {
            var query = _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.Recipient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.Status == status);

            return await query.ToListAsync();
        }

        public async Task<Donation?> GetByIdAsync(int id)
        {
            return await _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.Recipient)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task UpdateStatusAsync(int donationId, string status)
        {
            var donation = await _context.Donations.FindAsync(donationId);
            if (donation == null)
                throw new ArgumentException("Donation not found");

            donation.Status = status;
            donation.UpdatedAt = DateTime.UtcNow;

            if (status == "completed")
                donation.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "donation_status_updated", "Donation", donationId, $"Status updated to {status}");
        }
    }
}
