using NewLook.Models.DTOs.Api;

namespace NewLook.Services.Interfaces;

public interface IInventoryApiTokenService
{
    Task<InventoryApiTokenDto> GenerateTokenAsync(int inventoryId, int userId, CreateApiTokenDto dto);
    Task<List<InventoryApiTokenDto>> GetInventoryTokensAsync(int inventoryId, int userId);
    Task<bool> RevokeTokenAsync(int tokenId, int userId);
    Task<int?> ValidateTokenAsync(string token);
    Task UpdateLastUsedAsync(string token);
}
