using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VolunteersController : ControllerBase
    {
        private readonly WasteNautDbContext _context;

        public VolunteersController(WasteNautDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VolunteerDto>>> GetVolunteers([FromQuery] int? organizationId = null)
        {
            var query = _context.Volunteers.AsQueryable();
            
            if (organizationId.HasValue)
            {
                query = query.Where(v => v.OrganizationId == organizationId.Value);
            }

            var volunteers = await query
                .OrderBy(v => v.Name)
                .Select(v => new VolunteerDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Email = v.Email,
                    Phone = v.Phone,
                    Role = v.Role,
                    Skills = v.Skills,
                    StartDate = v.StartDate,
                    Status = v.Status,
                    OrganizationId = v.OrganizationId,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt
                })
                .ToListAsync();

            return Ok(volunteers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VolunteerDto>> GetVolunteer(int id)
        {
            var volunteer = await _context.Volunteers
                .Where(v => v.Id == id)
                .Select(v => new VolunteerDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Email = v.Email,
                    Phone = v.Phone,
                    Role = v.Role,
                    Skills = v.Skills,
                    StartDate = v.StartDate,
                    Status = v.Status,
                    OrganizationId = v.OrganizationId,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (volunteer == null)
            {
                return NotFound();
            }

            return Ok(volunteer);
        }

        [HttpPost]
        public async Task<ActionResult<VolunteerDto>> CreateVolunteer(CreateVolunteerDto createVolunteerDto)
        {
            var volunteer = new Volunteer
            {
                Name = createVolunteerDto.Name,
                Email = createVolunteerDto.Email,
                Phone = createVolunteerDto.Phone,
                Role = createVolunteerDto.Role,
                Skills = createVolunteerDto.Skills,
                StartDate = createVolunteerDto.StartDate,
                Status = createVolunteerDto.Status,
                OrganizationId = createVolunteerDto.OrganizationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();

            var volunteerDto = new VolunteerDto
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Email = volunteer.Email,
                Phone = volunteer.Phone,
                Role = volunteer.Role,
                Skills = volunteer.Skills,
                StartDate = volunteer.StartDate,
                Status = volunteer.Status,
                OrganizationId = volunteer.OrganizationId,
                CreatedAt = volunteer.CreatedAt,
                UpdatedAt = volunteer.UpdatedAt
            };

            return CreatedAtAction(nameof(GetVolunteer), new { id = volunteer.Id }, volunteerDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVolunteer(int id, UpdateVolunteerDto updateVolunteerDto)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            volunteer.Name = updateVolunteerDto.Name;
            volunteer.Email = updateVolunteerDto.Email;
            volunteer.Phone = updateVolunteerDto.Phone;
            volunteer.Role = updateVolunteerDto.Role;
            volunteer.Skills = updateVolunteerDto.Skills;
            volunteer.StartDate = updateVolunteerDto.StartDate;
            volunteer.Status = updateVolunteerDto.Status;
            volunteer.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolunteerExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            _context.Volunteers.Remove(volunteer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.Id == id);
        }
    }
}
