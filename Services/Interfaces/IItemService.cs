using NewLook.Models.DTOs.Item;

namespace NewLook.Services.Interfaces
{
    public interface IItemService
    {
        Task<(bool Success, string Message, ItemResponseDto? Data)> CreateItemAsync(CreateItemDto dto, int userId);
        Task<(bool Success, string Message, ItemResponseDto? Data)> UpdateItemAsync(int itemId, UpdateItemDto dto, int userId);
        Task<(bool Success, string Message)> DeleteItemAsync(int itemId, int userId);
        Task<ItemResponseDto?> GetItemByIdAsync(int itemId, int? userId);
        Task<List<ItemResponseDto>> GetItemsByInventoryIdAsync(int inventoryId, int? userId);
        Task<(bool Success, string Message)> LikeItemAsync(int itemId, int userId);
        Task<(bool Success, string Message)> UnlikeItemAsync(int itemId, int userId);
    }
}