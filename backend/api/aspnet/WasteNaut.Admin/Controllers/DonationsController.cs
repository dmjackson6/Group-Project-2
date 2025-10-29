using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationsController : ControllerBase
    {
        private readonly WasteNautDbContext _context;

        public DonationsController(WasteNautDbContext context)
        {
            _context = context;
        }

        // GET: api/donations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donation>>> GetDonations()
        {
            try
            {
                var donations = await _context.Donations
                    .Include(d => d.Donor)
                    .Include(d => d.Organization)
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync();
                return Ok(donations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving donations", error = ex.Message });
            }
        }

        // GET: api/donations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Donation>> GetDonation(int id)
        {
            try
            {
                var donation = await _context.Donations
                    .Include(d => d.Donor)
                    .Include(d => d.Organization)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (donation == null)
                {
                    return NotFound(new { message = "Donation not found" });
                }

                return Ok(donation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving donation", error = ex.Message });
            }
        }

        // POST: api/donations
        [HttpPost]
        public async Task<ActionResult<Donation>> CreateDonation([FromBody] CreateDonationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var donation = new Donation
                {
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Quantity = request.Quantity,
                    Unit = request.Unit,
                    ExpirationDate = request.ExpirationDate,
                    PickupLocation = request.PickupLocation,
                    Status = "available",
                    DonorId = request.DonorId,
                    OrganizationId = request.OrganizationId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDonation), new { id = donation.Id }, donation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating donation", error = ex.Message });
            }
        }

        // PUT: api/donations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonation(int id, [FromBody] UpdateDonationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var donation = await _context.Donations.FindAsync(id);
                if (donation == null)
                {
                    return NotFound(new { message = "Donation not found" });
                }

                // Update donation properties
                donation.Title = request.Title ?? donation.Title;
                donation.Description = request.Description ?? donation.Description;
                donation.Category = request.Category ?? donation.Category;
                donation.Quantity = request.Quantity ?? donation.Quantity;
                donation.Unit = request.Unit ?? donation.Unit;
                donation.ExpirationDate = request.ExpirationDate ?? donation.ExpirationDate;
                donation.PickupLocation = request.PickupLocation ?? donation.PickupLocation;
                donation.Status = request.Status ?? donation.Status;
                donation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Donation updated successfully", donation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating donation", error = ex.Message });
            }
        }

        // DELETE: api/donations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            try
            {
                var donation = await _context.Donations.FindAsync(id);
                if (donation == null)
                {
                    return NotFound(new { message = "Donation not found" });
                }

                _context.Donations.Remove(donation);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Donation deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting donation", error = ex.Message });
            }
        }

        // POST: api/donations/5/claim
        [HttpPost("{id}/claim")]
        public async Task<IActionResult> ClaimDonation(int id, [FromBody] ClaimDonationRequest request)
        {
            try
            {
                var donation = await _context.Donations.FindAsync(id);
                if (donation == null)
                {
                    return NotFound(new { message = "Donation not found" });
                }

                if (donation.Status != "available")
                {
                    return BadRequest(new { message = "Donation is not available for claiming" });
                }

                donation.Status = "claimed";
                donation.ClaimedBy = request.ClaimedById;
                donation.ClaimedAt = DateTime.UtcNow;
                donation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Donation claimed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error claiming donation", error = ex.Message });
            }
        }

        // GET: api/donations/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetDonationStats()
        {
            try
            {
                var totalDonations = await _context.Donations.CountAsync();
                var availableDonations = await _context.Donations.CountAsync(d => d.Status == "available");
                var claimedDonations = await _context.Donations.CountAsync(d => d.Status == "claimed");
                var completedDonations = await _context.Donations.CountAsync(d => d.Status == "completed");

                var categoryStats = await _context.Donations
                    .GroupBy(d => d.Category)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    totalDonations,
                    availableDonations,
                    claimedDonations,
                    completedDonations,
                    categoryStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving donation statistics", error = ex.Message });
            }
        }
    }

    public class CreateDonationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public int DonorId { get; set; }
        public int? OrganizationId { get; set; }
    }

    public class UpdateDonationRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? Quantity { get; set; }
        public string? Unit { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? PickupLocation { get; set; }
        public string? Status { get; set; }
    }

    public class ClaimDonationRequest
    {
        public int ClaimedById { get; set; }
    }
}
