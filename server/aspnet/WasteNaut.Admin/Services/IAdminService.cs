using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Services
{
    public interface IAdminService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<Admin?> GetByIdAsync(int id);
        Task<Admin?> GetByEmailAsync(string email);
        Task<bool> ValidatePasswordAsync(Admin admin, string password);
        Task LogActivityAsync(int adminId, string action, string target, int? targetId, string details);
    }
}
