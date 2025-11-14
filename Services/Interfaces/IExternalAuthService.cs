using NewLook.Models.DTOs.Auth;

namespace NewLook.Services.Interfaces
{
    public interface IExternalAuthService
    {
        Task<(bool Success, string Message, GoogleUserInfoDto? UserInfo)> ValidateGoogleTokenAsync(string accessToken);
        Task<(bool Success, string Message, GitHubUserInfoDto? UserInfo)> ValidateGitHubTokenAsync(string accessToken);
        Task<(bool Success, string Message, string? AccessToken)> ExchangeGitHubCodeAsync(string code);
    }
}
