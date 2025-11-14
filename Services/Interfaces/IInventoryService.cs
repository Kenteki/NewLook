using NewLook.Models.DTOs.Inventory;
using NewLook.Models.DTOs;
using NewLook.Services;

namespace NewLook.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<(bool Success, string Message, InventoryResponseDto? Data)> CreateInventoryAsync(CreateInventoryDto dto, int userId);
        Task<(bool Success, string Message, InventoryResponseDto? Data)> UpdateInventoryAsync(int inventoryId, UpdateInventoryDto dto, int userId);
        Task<(bool Success, string Message)> DeleteInventoryAsync(int inventoryId, int userId);
        Task<InventoryResponseDto?> GetInventoryByIdAsync(int inventoryId, int? userId);
        Task<List<InventoryListItemDto>> GetUserInventoriesAsync(int userId);
        Task<List<InventoryListItemDto>> GetInventoriesWithAccessAsync(int userId);
        Task<List<InventoryListItemDto>> GetPublicInventoriesAsync(int take = 10);
        Task<List<InventoryListItemDto>> GetLatestInventoriesAsync(int take = 5);
        Task<List<InventoryListItemDto>> GetPopularInventoriesAsync(int take = 5);
        Task<List<InventoryListItemDto>> SearchInventoriesAsync(string query);
        Task<List<InventoryAccessDto>> GetInventoryAccessListAsync(int inventoryId, int userId);
        Task<(bool Success, string Message)> GrantAccessAsync(int inventoryId, string emailOrUsername, int userId);
        Task<(bool Success, string Message)> RevokeAccessAsync(int inventoryId, int targetUserId, int userId);
        Task<InventoryStatsDto?> GetInventoryStatsAsync(int inventoryId);
        Task<bool> HasWriteAccessAsync(int inventoryId, int userId);
        Task<(bool Success, string Message)> SaveCustomIdConfigurationAsync(int inventoryId, List<CustomIdElementDto> elements, int userId);
        Task<List<CustomIdElementDto>> GetCustomIdConfigurationAsync(int inventoryId);
        Task<List<TagDto>> GetAllTagsAsync();
        Task<List<InventoryListItemDto>> GetInventoriesByTagAsync(string tagName);
    }
}
