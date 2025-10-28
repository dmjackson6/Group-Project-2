using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationProfileController : ControllerBase
    {
        private readonly WasteNautDbContext _context;

        public OrganizationProfileController(WasteNautDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationProfileDto>> GetOrganizationProfile(int id)
        {
            var organization = await _context.Organizations
                .Where(o => o.Id == id)
                .Select(o => new OrganizationProfileDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Type = o.Type,
                    ContactName = o.ContactName,
                    ContactEmail = o.ContactEmail,
                    ContactPhone = o.ContactPhone,
                    Website = o.ServiceAreas, // Using ServiceAreas field for website temporarily
                    Address = o.Address,
                    CapacityMax = o.CapacityMax,
                    OperatingHours = o.CapacityNotes, // Using CapacityNotes field for operating hours temporarily
                    Description = o.Description,
                    UpdatedAt = o.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (organization == null)
            {
                return NotFound();
            }

            return Ok(organization);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganizationProfile(int id, UpdateOrganizationProfileDto updateDto)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
            {
                return NotFound();
            }

            organization.Name = updateDto.Name;
            organization.Type = updateDto.Type;
            organization.ContactName = updateDto.ContactName;
            organization.ContactEmail = updateDto.ContactEmail;
            organization.ContactPhone = updateDto.ContactPhone;
            organization.ServiceAreas = updateDto.Website; // Using ServiceAreas field for website temporarily
            organization.Address = updateDto.Address;
            organization.CapacityMax = updateDto.CapacityMax;
            organization.CapacityNotes = updateDto.OperatingHours; // Using CapacityNotes field for operating hours temporarily
            organization.Description = updateDto.Description;
            organization.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}
