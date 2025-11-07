using NewLook.Models.DTOs.Auth;
using NewLook.Models.Entities;

namespace NewLook.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, AuthResponseDto? Response)> RegisterAsync(RegisterRequestDto dto);
        Task<(bool Success, string Message, AuthResponseDto? Response)> LoginAsync(LoginRequestDto dto);
        Task<(bool Success, string Message, AuthResponseDto? Response)> ExternalLoginAsync(string provider, string ProviderID, string email, string username);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto dto);
        Task<(bool Success, string Message)> VerifyEmailAsync(string token);
        Task<User?> GetUserByIdAsync(int userId);
    }
}
