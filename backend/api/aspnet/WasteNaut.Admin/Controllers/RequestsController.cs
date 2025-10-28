using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly WasteNautDbContext _context;

        public RequestsController(WasteNautDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetRequests([FromQuery] int? organizationId = null)
        {
            var query = _context.Requests.AsQueryable();
            
            if (organizationId.HasValue)
            {
                query = query.Where(r => r.OrganizationId == organizationId.Value);
            }

            var requests = await query
                .OrderByDescending(r => r.RequestDate)
                .Select(r => new RequestDto
                {
                    Id = r.Id,
                    Requester = r.Requester,
                    Items = r.Items,
                    Priority = r.Priority,
                    Status = r.Status,
                    RequestDate = r.RequestDate,
                    OrganizationId = r.OrganizationId,
                    Notes = r.Notes,
                    CompletedAt = r.CompletedAt,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestDto>> GetRequest(int id)
        {
            var request = await _context.Requests
                .Where(r => r.Id == id)
                .Select(r => new RequestDto
                {
                    Id = r.Id,
                    Requester = r.Requester,
                    Items = r.Items,
                    Priority = r.Priority,
                    Status = r.Status,
                    RequestDate = r.RequestDate,
                    OrganizationId = r.OrganizationId,
                    Notes = r.Notes,
                    CompletedAt = r.CompletedAt,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        [HttpPost]
        public async Task<ActionResult<RequestDto>> CreateRequest(CreateRequestDto createRequestDto)
        {
            var request = new Request
            {
                Requester = createRequestDto.Requester,
                Items = createRequestDto.Items,
                Priority = createRequestDto.Priority,
                Status = createRequestDto.Status,
                RequestDate = DateTime.UtcNow,
                OrganizationId = createRequestDto.OrganizationId,
                Notes = createRequestDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var requestDto = new RequestDto
            {
                Id = request.Id,
                Requester = request.Requester,
                Items = request.Items,
                Priority = request.Priority,
                Status = request.Status,
                RequestDate = request.RequestDate,
                OrganizationId = request.OrganizationId,
                Notes = request.Notes,
                CompletedAt = request.CompletedAt,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt
            };

            return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, requestDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(int id, UpdateRequestDto updateRequestDto)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Requester = updateRequestDto.Requester;
            request.Items = updateRequestDto.Items;
            request.Priority = updateRequestDto.Priority;
            request.Status = updateRequestDto.Status;
            request.Notes = updateRequestDto.Notes;
            request.UpdatedAt = DateTime.UtcNow;

            // If status is completed, set completed date
            if (updateRequestDto.Status == "Completed" && request.CompletedAt == null)
            {
                request.CompletedAt = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
