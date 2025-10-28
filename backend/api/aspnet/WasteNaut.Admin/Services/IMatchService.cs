using WasteNaut.Admin.Models;
using WasteNaut.Admin.DTOs;

namespace WasteNaut.Admin.Services
{
    public interface IMatchService
    {
        Task<IEnumerable<Match>> GetMatchesAsync(string? status, string? confidence);
        Task<Match?> GetByIdAsync(int id);
        Task AcceptMatchAsync(int matchId);
        Task RejectMatchAsync(int matchId, string reason);
        Task OverrideMatchAsync(int matchId, string reason, string notes, int newConfidence);
        Task<GenerateMatchesResponseDto> GenerateMatchesAsync();
    }
}
