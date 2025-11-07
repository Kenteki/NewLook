using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.DTOs.Inventory.Interfaces;
using NewLook.Models.DTOs.Item;
using NewLook.Models.Entities;
using NewLook.Services.Interfaces;

namespace NewLook.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomIdService _customIdService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<ItemService> _logger;

        public ItemService(
            ApplicationDbContext context,
            ICustomIdService customIdService,
            IInventoryService inventoryService,
            ILogger<ItemService> logger)
        {
            _context = context;
            _customIdService = customIdService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, ItemResponseDto? Data)> CreateItemAsync(CreateItemDto dto, int userId)
        {
            try
            {
                // Check write access
                var hasAccess = await _inventoryService.HasWriteAccessAsync(dto.InventoryId, userId);
                if (!hasAccess)
                    return (false, "You don't have permission to add items to this inventory", null);

                // Generate or validate custom ID
                string customId;
                if (string.IsNullOrEmpty(dto.CustomId))
                {
                    customId = await _customIdService.GenerateCustomIdAsync(dto.InventoryId);
                }
                else
                {
                    customId = dto.CustomId;
                    // Check uniqueness
                    var exists = await _context.Items
                        .AnyAsync(i => i.InventoryId == dto.InventoryId && i.CustomId == customId);
                    
                    if (exists)
                        return (false, "An item with this custom ID already exists in this inventory", null);
                }

                var item = new Item
                {
                    InventoryId = dto.InventoryId,
                    CustomId = customId,
                    CustomString1Value = dto.CustomString1Value,
                    CustomString2Value = dto.CustomString2Value,
                    CustomString3Value = dto.CustomString3Value,
                    CustomText1Value = dto.CustomText1Value,
                    CustomText2Value = dto.CustomText2Value,
                    CustomText3Value = dto.CustomText3Value,
                    CustomNumber1Value = dto.CustomNumber1Value,
                    CustomNumber2Value = dto.CustomNumber2Value,
                    CustomNumber3Value = dto.CustomNumber3Value,
                    CustomLink1Value = dto.CustomLink1Value,
                    CustomLink2Value = dto.CustomLink2Value,
                    CustomLink3Value = dto.CustomLink3Value,
                    CustomBool1Value = dto.CustomBool1Value,
                    CustomBool2Value = dto.CustomBool2Value,
                    CustomBool3Value = dto.CustomBool3Value,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                var result = await GetItemByIdAsync(item.Id, userId);
                return (true, "Item created successfully", result);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                return (false, "An item with this custom ID already exists. Please try again.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                return (false, "Error creating item", null);
            }
        }

        public async Task<(bool Success, string Message, ItemResponseDto? Data)> UpdateItemAsync(int itemId, UpdateItemDto dto, int userId)
        {
            try
            {
                var item = await _context.Items
                    .Include(i => i.Inventory)
                    .FirstOrDefaultAsync(i => i.Id == itemId);

                if (item == null)
                    return (false, "Item not found", null);

                // Check write access
                var hasAccess = await _inventoryService.HasWriteAccessAsync(item.InventoryId, userId);
                if (!hasAccess)
                    return (false, "You don't have permission to edit this item", null);

                // Optimistic locking check
                if (item.Version != dto.Version)
                    return (false, "This item was modified by someone else. Please reload and try again.", null);

                // Update custom ID if changed
                if (!string.IsNullOrEmpty(dto.CustomId) && dto.CustomId != item.CustomId)
                {
                    var exists = await _context.Items
                        .AnyAsync(i => i.InventoryId == item.InventoryId && i.CustomId == dto.CustomId && i.Id != itemId);
                    
                    if (exists)
                        return (false, "An item with this custom ID already exists in this inventory", null);

                    item.CustomId = dto.CustomId;
                }

                item.CustomString1Value = dto.CustomString1Value;
                item.CustomString2Value = dto.CustomString2Value;
                item.CustomString3Value = dto.CustomString3Value;
                item.CustomText1Value = dto.CustomText1Value;
                item.CustomText2Value = dto.CustomText2Value;
                item.CustomText3Value = dto.CustomText3Value;
                item.CustomNumber1Value = dto.CustomNumber1Value;
                item.CustomNumber2Value = dto.CustomNumber2Value;
                item.CustomNumber3Value = dto.CustomNumber3Value;
                item.CustomLink1Value = dto.CustomLink1Value;
                item.CustomLink2Value = dto.CustomLink2Value;
                item.CustomLink3Value = dto.CustomLink3Value;
                item.CustomBool1Value = dto.CustomBool1Value;
                item.CustomBool2Value = dto.CustomBool2Value;
                item.CustomBool3Value = dto.CustomBool3Value;
                item.UpdatedAt = DateTime.UtcNow;
                item.Version++;

                await _context.SaveChangesAsync();

                var result = await GetItemByIdAsync(item.Id, userId);
                return (true, "Item updated successfully", result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "This item was modified by someone else. Please reload and try again.", null);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                return (false, "An item with this custom ID already exists. Please try again.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item");
                return (false, "Error updating item", null);
            }
        }

        public async Task<(bool Success, string Message)> DeleteItemAsync(int itemId, int userId)
        {
            try
            {
                var item = await _context.Items
                    .Include(i => i.Inventory)
                    .FirstOrDefaultAsync(i => i.Id == itemId);

                if (item == null)
                    return (false, "Item not found");

                // Check write access
                var hasAccess = await _inventoryService.HasWriteAccessAsync(item.InventoryId, userId);
                if (!hasAccess)
                    return (false, "You don't have permission to delete this item");

                _context.Items.Remove(item);
                await _context.SaveChangesAsync();

                return (true, "Item deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item");
                return (false, "Error deleting item");
            }
        }

        public async Task<ItemResponseDto?> GetItemByIdAsync(int itemId, int? userId)
        {
            var item = await _context.Items
                .Include(i => i.CreatedBy)
                .Include(i => i.Likes)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
                return null;

            bool isLiked = false;
            if (userId.HasValue)
            {
                isLiked = await _context.Likes
                    .AnyAsync(l => l.ItemId == itemId && l.UserId == userId.Value);
            }

            return new ItemResponseDto
            {
                Id = item.Id,
                InventoryId = item.InventoryId,
                CustomId = item.CustomId,
                CustomString1Value = item.CustomString1Value,
                CustomString2Value = item.CustomString2Value,
                CustomString3Value = item.CustomString3Value,
                CustomText1Value = item.CustomText1Value,
                CustomText2Value = item.CustomText2Value,
                CustomText3Value = item.CustomText3Value,
                CustomNumber1Value = item.CustomNumber1Value,
                CustomNumber2Value = item.CustomNumber2Value,
                CustomNumber3Value = item.CustomNumber3Value,
                CustomLink1Value = item.CustomLink1Value,
                CustomLink2Value = item.CustomLink2Value,
                CustomLink3Value = item.CustomLink3Value,
                CustomBool1Value = item.CustomBool1Value,
                CustomBool2Value = item.CustomBool2Value,
                CustomBool3Value = item.CustomBool3Value,
                CreatedById = item.CreatedById,
                CreatedByUsername = item.CreatedBy.Username,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Version = item.Version,
                LikeCount = item.Likes.Count,
                IsLikedByCurrentUser = isLiked
            };
        }

        public async Task<List<ItemResponseDto>> GetItemsByInventoryIdAsync(int inventoryId, int? userId)
        {
            var items = await _context.Items
                .Include(i => i.CreatedBy)
                .Include(i => i.Likes)
                .Where(i => i.InventoryId == inventoryId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var likedItemIds = new HashSet<int>();
            if (userId.HasValue)
            {
                likedItemIds = (await _context.Likes
                    .Where(l => l.UserId == userId.Value && l.Item.InventoryId == inventoryId)
                    .Select(l => l.ItemId)
                    .ToListAsync())
                    .ToHashSet();
            }

            return items.Select(item => new ItemResponseDto
            {
                Id = item.Id,
                InventoryId = item.InventoryId,
                CustomId = item.CustomId,
                CustomString1Value = item.CustomString1Value,
                CustomString2Value = item.CustomString2Value,
                CustomString3Value = item.CustomString3Value,
                CustomText1Value = item.CustomText1Value,
                CustomText2Value = item.CustomText2Value,
                CustomText3Value = item.CustomText3Value,
                CustomNumber1Value = item.CustomNumber1Value,
                CustomNumber2Value = item.CustomNumber2Value,
                CustomNumber3Value = item.CustomNumber3Value,
                CustomLink1Value = item.CustomLink1Value,
                CustomLink2Value = item.CustomLink2Value,
                CustomLink3Value = item.CustomLink3Value,
                CustomBool1Value = item.CustomBool1Value,
                CustomBool2Value = item.CustomBool2Value,
                CustomBool3Value = item.CustomBool3Value,
                CreatedById = item.CreatedById,
                CreatedByUsername = item.CreatedBy.Username,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Version = item.Version,
                LikeCount = item.Likes.Count,
                IsLikedByCurrentUser = likedItemIds.Contains(item.Id)
            }).ToList();
        }

        public async Task<(bool Success, string Message)> LikeItemAsync(int itemId, int userId)
        {
            try
            {
                var item = await _context.Items.FindAsync(itemId);
                if (item == null)
                    return (false, "Item not found");

                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(l => l.ItemId == itemId && l.UserId == userId);

                if (existingLike != null)
                    return (false, "You already liked this item");

                _context.Likes.Add(new Like
                {
                    ItemId = itemId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                return (true, "Item liked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking item");
                return (false, "Error liking item");
            }
        }

        public async Task<(bool Success, string Message)> UnlikeItemAsync(int itemId, int userId)
        {
            try
            {
                var like = await _context.Likes
                    .FirstOrDefaultAsync(l => l.ItemId == itemId && l.UserId == userId);

                if (like == null)
                    return (false, "Like not found");

                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();

                return (true, "Item unliked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unliking item");
                return (false, "Error unliking item");
            }
        }
    }
}