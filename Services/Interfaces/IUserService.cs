using NewLook.Models.DTOs.Admin;

namespace NewLook.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserManagementDto>> GetAllUsersAsync();
        Task<(bool Success, string Message)> BlockUserAsync(int userId, int adminId);
        Task<(bool Success, string Message)> UnblockUserAsync(int userId, int adminId);
        Task<(bool Success, string Message)> DeleteUserAsync(int userId, int adminId);
        Task<(bool Success, string Message)> AddAdminRoleAsync(int userId, int adminId);
        Task<(bool Success, string Message)> RemoveAdminRoleAsync(int userId, int adminId);
        Task<bool> IsUserAdminAsync(int userId);
        Task<List<UserSearchDto>> SearchUsersAsync(string query);
    }
}
