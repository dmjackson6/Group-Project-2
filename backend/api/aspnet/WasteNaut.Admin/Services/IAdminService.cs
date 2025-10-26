using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;
using AdminModel = WasteNaut.Admin.Models.Admin;

namespace WasteNaut.Admin.Services
{
    public interface IAdminService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<AdminModel?> GetByIdAsync(int id);
        Task<AdminModel?> GetByEmailAsync(string email);
        Task<bool> ValidatePasswordAsync(AdminModel admin, string password);
        Task LogActivityAsync(int adminId, string action, string target, int? targetId, string details);
    }
}
