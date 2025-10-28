using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync(int page, int limit, string? status, string? role, string? search);
        Task<User?> GetByIdAsync(int id);
        Task VerifyUserAsync(int userId);
        Task SuspendUserAsync(int userId, string reason);
        Task UnsuspendUserAsync(int userId);
        Task<ImpersonationResponseDto> ImpersonateUserAsync(int userId);
    }
}
