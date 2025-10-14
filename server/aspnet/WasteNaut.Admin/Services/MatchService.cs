using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public class MatchService : IMatchService
    {
        private readonly WasteNautDbContext _context;
        private readonly IAuditService _auditService;

        public MatchService(WasteNautDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Match>> GetMatchesAsync(string? status, string? confidence)
        {
            var query = _context.Matches
                .Include(m => m.FromUser)
                .Include(m => m.ToUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(m => m.Status == status);

            if (!string.IsNullOrEmpty(confidence))
            {
                switch (confidence.ToLower())
                {
                    case "high":
                        query = query.Where(m => m.Confidence >= 90);
                        break;
                    case "medium":
                        query = query.Where(m => m.Confidence >= 70 && m.Confidence < 90);
                        break;
                    case "low":
                        query = query.Where(m => m.Confidence < 70);
                        break;
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Match?> GetByIdAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.FromUser)
                .Include(m => m.ToUser)
                .Include(m => m.Notes)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AcceptMatchAsync(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                throw new ArgumentException("Match not found");

            if (match.Status != "pending")
                throw new ArgumentException("Match is not pending");

            match.Status = "accepted";
            match.AcceptedAt = DateTime.UtcNow;
            match.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "match_accepted", "Match", matchId, "Match accepted");
        }

        public async Task RejectMatchAsync(int matchId, string reason)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                throw new ArgumentException("Match not found");

            if (match.Status != "pending")
                throw new ArgumentException("Match is not pending");

            match.Status = "rejected";
            match.RejectedAt = DateTime.UtcNow;
            match.RejectionReason = reason;
            match.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "match_rejected", "Match", matchId, $"Match rejected: {reason}");
        }

        public async Task OverrideMatchAsync(int matchId, string reason, string notes, int newConfidence)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                throw new ArgumentException("Match not found");

            match.OverrideReason = reason;
            match.OverrideNotes = notes;
            match.OverrideConfidence = newConfidence;
            match.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.LogActivityAsync(0, "match_override", "Match", matchId, $"Match overridden: {reason}");
        }

        public async Task<GenerateMatchesResponseDto> GenerateMatchesAsync()
        {
            // In real implementation, this would call AI service to generate matches
            // For now, return mock response
            var count = await Task.FromResult(5); // Mock: generate 5 new matches
            
            await _auditService.LogActivityAsync(0, "matches_generated", "Matches", 0, $"Generated {count} new matches");
            
            return new GenerateMatchesResponseDto
            {
                Count = count,
                Message = $"Generated {count} new matches successfully"
            };
        }
    }
}
