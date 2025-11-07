using NewLook.Models.Entities;
using System.Security.Claims;

namespace NewLook.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user, List<string> roles);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
