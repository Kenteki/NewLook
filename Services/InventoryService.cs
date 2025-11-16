using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.DTOs;
using NewLook.Models.DTOs.Inventory;
using NewLook.Models.Entities;
using NewLook.Services.Interfaces;

namespace NewLook.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(ApplicationDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, InventoryResponseDto? Data)> CreateInventoryAsync(CreateInventoryDto dto, int userId)
        {
            try
            {
                var inventory = new Inventory
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    CreatorId = userId,
                    CategoryId = dto.CategoryId,
                    IsPublic = dto.IsPublic,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Map custom fields
                MapCustomFieldsToInventory(dto.CustomFields, inventory);

                _context.Inventories.Add(inventory);

                // Add tags
                foreach (var tagName in dto.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        _context.Tags.Add(tag);
                    }

                    _context.InventoryTags.Add(new InventoryTag
                    {
                        Inventory = inventory,
                        Tag = tag
                    });
                }

                await _context.SaveChangesAsync();

                var result = await GetInventoryByIdAsync(inventory.Id, userId);
                return (true, "Inventory created successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory");
                return (false, "Error creating inventory", null);
            }
        }

        public async Task<(bool Success, string Message, InventoryResponseDto? Data)> UpdateInventoryAsync(int inventoryId, UpdateInventoryDto dto, int userId)
        {
            try
            {
                var inventory = await _context.Inventories
                    .Include(i => i.InventoryTags)
                    .FirstOrDefaultAsync(i => i.Id == inventoryId);

                if (inventory == null)
                    return (false, "Inventory not found", null);

                // Check permissions
                var isAdmin = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin");
                if (inventory.CreatorId != userId && !isAdmin)
                    return (false, "You don't have permission to edit this inventory", null);

                // Optimistic locking check
                if (inventory.Version != dto.Version)
                    return (false, "This inventory was modified by someone else. Please reload and try again.", null);

                inventory.Title = dto.Title;
                inventory.Description = dto.Description;
                inventory.ImageUrl = dto.ImageUrl;
                inventory.CategoryId = dto.CategoryId;
                inventory.IsPublic = dto.IsPublic;
                inventory.UpdatedAt = DateTime.UtcNow;
                inventory.Version++;

                // Update custom fields
                MapCustomFieldsToInventory(dto.CustomFields, inventory);

                // Update tags
                _context.InventoryTags.RemoveRange(inventory.InventoryTags);
                foreach (var tagName in dto.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        _context.Tags.Add(tag);
                    }

                    _context.InventoryTags.Add(new InventoryTag
                    {
                        InventoryId = inventory.Id,
                        Tag = tag
                    });
                }

                await _context.SaveChangesAsync();

                var result = await GetInventoryByIdAsync(inventory.Id, userId);
                return (true, "Inventory updated successfully", result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "This inventory was modified by someone else. Please reload and try again.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory");
                return (false, "Error updating inventory", null);
            }
        }

        public async Task<(bool Success, string Message)> DeleteInventoryAsync(int inventoryId, int userId)
        {
            try
            {
                var inventory = await _context.Inventories.FindAsync(inventoryId);
                if (inventory == null)
                    return (false, "Inventory not found");

                var isAdmin = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin");
                if (inventory.CreatorId != userId && !isAdmin)
                    return (false, "You don't have permission to delete this inventory");

                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();

                return (true, "Inventory deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory");
                return (false, "Error deleting inventory");
            }
        }

        public async Task<InventoryResponseDto?> GetInventoryByIdAsync(int inventoryId, int? userId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
                return null;

            var hasWriteAccess = userId.HasValue && await HasWriteAccessAsync(inventoryId, userId.Value);

            return new InventoryResponseDto
            {
                Id = inventory.Id,
                Title = inventory.Title,
                Description = inventory.Description,
                ImageUrl = inventory.ImageUrl,
                CreatorId = inventory.CreatorId,
                CreatorUsername = inventory.Creator.Username,
                CategoryId = inventory.CategoryId,
                CategoryName = inventory.Category?.Name,
                IsPublic = inventory.IsPublic,
                Tags = inventory.InventoryTags.Select(it => it.Tag.Name).ToList(),
                CustomFields = GetCustomFieldsFromInventory(inventory),
                ItemCount = inventory.Items.Count,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt,
                Version = inventory.Version,
                HasWriteAccess = hasWriteAccess
            };
        }

        public async Task<List<InventoryListItemDto>> GetUserInventoriesAsync(int userId)
        {
            return await _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(i => i.Items)
                .Where(i => i.CreatorId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new InventoryListItemDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description.Length > 150 ? i.Description.Substring(0, 150) + "..." : i.Description,
                    ImageUrl = i.ImageUrl,
                    CreatorUsername = i.Creator.Username,
                    CategoryName = i.Category != null ? i.Category.Name : null,
                    Tags = i.InventoryTags.Select(it => it.Tag.Name).ToList(),
                    ItemCount = i.Items.Count,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<InventoryListItemDto>> GetInventoriesWithAccessAsync(int userId)
        {
            return await _context.InventoryAccesses
                .Include(ia => ia.Inventory).ThenInclude(i => i.Creator)
                .Include(ia => ia.Inventory).ThenInclude(i => i.Category)
                .Include(ia => ia.Inventory).ThenInclude(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(ia => ia.Inventory).ThenInclude(i => i.Items)
                .Where(ia => ia.UserId == userId)
                .Select(ia => new InventoryListItemDto
                {
                    Id = ia.Inventory.Id,
                    Title = ia.Inventory.Title,
                    Description = ia.Inventory.Description.Length > 150 ? ia.Inventory.Description.Substring(0, 150) + "..." : ia.Inventory.Description,
                    ImageUrl = ia.Inventory.ImageUrl,
                    CreatorUsername = ia.Inventory.Creator.Username,
                    CategoryName = ia.Inventory.Category != null ? ia.Inventory.Category.Name : null,
                    Tags = ia.Inventory.InventoryTags.Select(it => it.Tag.Name).ToList(),
                    ItemCount = ia.Inventory.Items.Count,
                    CreatedAt = ia.Inventory.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<InventoryListItemDto>> GetPublicInventoriesAsync(int take = 10)
        {
            var inventories = await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(i => i.Items)
                .Where(i => i.IsPublic)
                .OrderByDescending(i => i.CreatedAt)
                .Take(take)
                .ToListAsync();

            return inventories.Select(i => new InventoryListItemDto
            {
                Id = i.Id,
                Title = i.Title ?? "",
                Description = !string.IsNullOrEmpty(i.Description) && i.Description.Length > 150
                    ? i.Description.Substring(0, 150) + "..."
                    : i.Description ?? "",
                ImageUrl = i.ImageUrl,
                CreatorUsername = i.Creator?.Username ?? "Unknown",
                CategoryName = i.Category?.Name,
                Tags = i.InventoryTags?.Select(it => it.Tag?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>(),
                ItemCount = i.Items?.Count ?? 0,
                CreatedAt = i.CreatedAt
            }).ToList();
        }

        public async Task<List<InventoryListItemDto>> GetLatestInventoriesAsync(int take = 5)
        {
            var inventories = await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(i => i.Items)
                .OrderByDescending(i => i.CreatedAt)
                .Take(take)
                .ToListAsync();

            return inventories.Select(i => new InventoryListItemDto
            {
                Id = i.Id,
                Title = i.Title ?? "",
                Description = !string.IsNullOrEmpty(i.Description) && i.Description.Length > 150
                    ? i.Description.Substring(0, 150) + "..."
                    : i.Description ?? "",
                ImageUrl = i.ImageUrl,
                CreatorUsername = i.Creator?.Username ?? "Unknown",
                CategoryName = i.Category?.Name,
                Tags = i.InventoryTags?.Select(it => it.Tag?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>(),
                ItemCount = i.Items?.Count ?? 0,
                CreatedAt = i.CreatedAt
            }).ToList();
        }

        public async Task<List<InventoryListItemDto>> GetPopularInventoriesAsync(int take = 5)
        {
            // Load all inventories with their related data
            var inventories = await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(i => i.Items)
                .ToListAsync();

            // Sort by item count in memory and take top N
            var popularInventories = inventories
                .OrderByDescending(i => i.Items?.Count ?? 0)
                .Take(take)
                .ToList();

            return popularInventories.Select(i => new InventoryListItemDto
            {
                Id = i.Id,
                Title = i.Title ?? "",
                Description = !string.IsNullOrEmpty(i.Description) && i.Description.Length > 150
                    ? i.Description.Substring(0, 150) + "..."
                    : i.Description ?? "",
                ImageUrl = i.ImageUrl,
                CreatorUsername = i.Creator?.Username ?? "Unknown",
                CategoryName = i.Category?.Name,
                Tags = i.InventoryTags?.Select(it => it.Tag?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>(),
                ItemCount = i.Items?.Count ?? 0,
                CreatedAt = i.CreatedAt
            }).ToList();
        }

        public async Task<List<InventoryListItemDto>> SearchInventoriesAsync(string query)
        {
            var lowerQuery = query.ToLower();

            var inventories = await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(i => i.Items)
                .Where(i => i.Title.ToLower().Contains(lowerQuery) ||
                           (i.Description != null && i.Description.ToLower().Contains(lowerQuery)) ||
                           i.InventoryTags.Any(it => it.Tag.Name.ToLower().Contains(lowerQuery)))
                .ToListAsync();

            return inventories.Select(i => new InventoryListItemDto
            {
                Id = i.Id,
                Title = i.Title ?? "",
                Description = !string.IsNullOrEmpty(i.Description) && i.Description.Length > 150
                    ? i.Description.Substring(0, 150) + "..."
                    : i.Description ?? "",
                ImageUrl = i.ImageUrl,
                CreatorUsername = i.Creator?.Username ?? "Unknown",
                CategoryName = i.Category?.Name,
                Tags = i.InventoryTags?.Select(it => it.Tag?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>(),
                ItemCount = i.Items?.Count ?? 0,
                CreatedAt = i.CreatedAt
            }).ToList();
        }

        public async Task<List<InventoryAccessDto>> GetInventoryAccessListAsync(int inventoryId, int userId)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null || inventory.CreatorId != userId)
                return new List<InventoryAccessDto>();

            return await _context.InventoryAccesses
                .Include(ia => ia.User)
                .Where(ia => ia.InventoryId == inventoryId)
                .Select(ia => new InventoryAccessDto
                {
                    UserId = ia.UserId,
                    Username = ia.User.Username,
                    Email = ia.User.Email,
                    GrantedAt = ia.GrantedAt
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> GrantAccessAsync(int inventoryId, string emailOrUsername, int userId)
        {
            try
            {
                var inventory = await _context.Inventories.FindAsync(inventoryId);
                if (inventory == null)
                    return (false, "Inventory not found");

                if (inventory.CreatorId != userId)
                    return (false, "Only the creator can grant access");

                var targetUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

                if (targetUser == null)
                    return (false, "User not found");

                if (targetUser.Id == userId)
                    return (false, "You already have access as the creator");

                var existingAccess = await _context.InventoryAccesses
                    .AnyAsync(ia => ia.InventoryId == inventoryId && ia.UserId == targetUser.Id);

                if (existingAccess)
                    return (false, "User already has access");

                _context.InventoryAccesses.Add(new InventoryAccess
                {
                    InventoryId = inventoryId,
                    UserId = targetUser.Id,
                    GrantedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                return (true, "Access granted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error granting access");
                return (false, "Error granting access");
            }
        }

        public async Task<(bool Success, string Message)> RevokeAccessAsync(int inventoryId, int targetUserId, int userId)
        {
            try
            {
                var inventory = await _context.Inventories.FindAsync(inventoryId);
                if (inventory == null)
                    return (false, "Inventory not found");

                if (inventory.CreatorId != userId)
                    return (false, "Only the creator can revoke access");

                var access = await _context.InventoryAccesses
                    .FirstOrDefaultAsync(ia => ia.InventoryId == inventoryId && ia.UserId == targetUserId);

                if (access == null)
                    return (false, "Access not found");

                _context.InventoryAccesses.Remove(access);
                await _context.SaveChangesAsync();

                return (true, "Access revoked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking access");
                return (false, "Error revoking access");
            }
        }

        public async Task<InventoryStatsDto?> GetInventoryStatsAsync(int inventoryId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
                return null;

            var stats = new InventoryStatsDto
            {
                TotalItems = inventory.Items.Count
            };

            // Calculate numeric field statistics
            for (int i = 1; i <= 3; i++)
            {
                var fieldName = $"CustomNumber{i}Name";
                var fieldEnabled = $"CustomNumber{i}Enabled";
                
                var propertyName = typeof(Inventory).GetProperty(fieldName)?.GetValue(inventory) as string;
                var isEnabled = (bool)(typeof(Inventory).GetProperty(fieldEnabled)?.GetValue(inventory) ?? false);

                if (isEnabled && !string.IsNullOrEmpty(propertyName))
                {
                    var values = inventory.Items
                        .Select(item => typeof(Item).GetProperty($"CustomNumber{i}Value")?.GetValue(item) as decimal?)
                        .Where(v => v.HasValue)
                        .Select(v => v!.Value)
                        .ToList();

                    if (values.Any())
                    {
                        stats.FieldStats[propertyName] = new NumericFieldStats
                        {
                            Min = values.Min(),
                            Max = values.Max(),
                            Average = values.Average(),
                            Count = values.Count
                        };
                    }
                }
            }

            // Calculate string field statistics
            for (int i = 1; i <= 3; i++)
            {
                var fieldName = $"CustomString{i}Name";
                var fieldEnabled = $"CustomString{i}Enabled";
                
                var propertyName = typeof(Inventory).GetProperty(fieldName)?.GetValue(inventory) as string;
                var isEnabled = (bool)(typeof(Inventory).GetProperty(fieldEnabled)?.GetValue(inventory) ?? false);

                if (isEnabled && !string.IsNullOrEmpty(propertyName))
                {
                    var values = inventory.Items
                        .Select(item => typeof(Item).GetProperty($"CustomString{i}Value")?.GetValue(item) as string)
                        .Where(v => !string.IsNullOrEmpty(v))
                        .ToList();

                    if (values.Any())
                    {
                        var grouped = values
                            .GroupBy(v => v)
                            .OrderByDescending(g => g.Count())
                            .Take(5)
                            .Select(g => new ValueCount { Value = g.Key!, Count = g.Count() })
                            .ToList();

                        stats.FieldStats[propertyName] = new StringFieldStats
                        {
                            TopValues = grouped,
                            UniqueCount = values.Distinct().Count()
                        };
                    }
                }
            }

            return stats;
        }

        public async Task<bool> HasWriteAccessAsync(int inventoryId, int userId)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null)
                return false;

            // Creator always has access
            if (inventory.CreatorId == userId)
                return true;

            // Admin always has access
            var isAdmin = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin");
            if (isAdmin)
                return true;

            // Public inventory
            if (inventory.IsPublic)
                return true;

            // Explicit access grant
            return await _context.InventoryAccesses
                .AnyAsync(ia => ia.InventoryId == inventoryId && ia.UserId == userId);
        }

        // Helper methods
        private void MapCustomFieldsToInventory(List<CustomFieldDto> fields, Inventory inventory)
        {
            int stringIndex = 1, textIndex = 1, numberIndex = 1, linkIndex = 1, boolIndex = 1;

            foreach (var field in fields)
            {
                switch (field.FieldType.ToLower())
                {
                    case "string" when stringIndex <= 3:
                        typeof(Inventory).GetProperty($"CustomString{stringIndex}Enabled")?.SetValue(inventory, true);
                        typeof(Inventory).GetProperty($"CustomString{stringIndex}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"CustomString{stringIndex}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"CustomString{stringIndex}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        stringIndex++;
                        break;

                    case "text" when textIndex <= 3:
                        typeof(Inventory).GetProperty($"CustomText{textIndex}Enabled")?.SetValue(inventory, true);
                        typeof(Inventory).GetProperty($"CustomText{textIndex}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"CustomText{textIndex}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"CustomText{textIndex}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        textIndex++;
                        break;

                    case "number" when numberIndex <= 3:
                        typeof(Inventory).GetProperty($"CustomNumber{numberIndex}Enabled")?.SetValue(inventory, true);
                        typeof(Inventory).GetProperty($"CustomNumber{numberIndex}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"CustomNumber{numberIndex}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"CustomNumber{numberIndex}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        numberIndex++;
                        break;

                    case "link" when linkIndex <= 3:
                        typeof(Inventory).GetProperty($"CustomLink{linkIndex}Enabled")?.SetValue(inventory, true);
                        typeof(Inventory).GetProperty($"CustomLink{linkIndex}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"CustomLink{linkIndex}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"CustomLink{linkIndex}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        linkIndex++;
                        break;

                    case "bool" when boolIndex <= 3:
                        typeof(Inventory).GetProperty($"CustomBool{boolIndex}Enabled")?.SetValue(inventory, true);
                        typeof(Inventory).GetProperty($"CustomBool{boolIndex}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"CustomBool{boolIndex}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"CustomBool{boolIndex}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        boolIndex++;
                        break;
                }
            }
        }

        private List<CustomFieldDto> GetCustomFieldsFromInventory(Inventory inventory)
        {
            var fields = new List<CustomFieldDto>();

            for (int i = 1; i <= 3; i++)
            {
                // String fields
                if ((bool)(typeof(Inventory).GetProperty($"CustomString{i}Enabled")?.GetValue(inventory) ?? false))
                {
                    fields.Add(new CustomFieldDto
                    {
                        FieldType = "string",
                        Name = typeof(Inventory).GetProperty($"CustomString{i}Name")?.GetValue(inventory) as string ?? "",
                        Description = typeof(Inventory).GetProperty($"CustomString{i}Description")?.GetValue(inventory) as string,
                        ShowInTable = (bool)(typeof(Inventory).GetProperty($"CustomString{i}ShowInTable")?.GetValue(inventory) ?? false),
                        SlotNumber = i
                    });
                }

                // Text fields
                if ((bool)(typeof(Inventory).GetProperty($"CustomText{i}Enabled")?.GetValue(inventory) ?? false))
                {
                    fields.Add(new CustomFieldDto
                    {
                        FieldType = "text",
                        Name = typeof(Inventory).GetProperty($"CustomText{i}Name")?.GetValue(inventory) as string ?? "",
                        Description = typeof(Inventory).GetProperty($"CustomText{i}Description")?.GetValue(inventory) as string,
                        ShowInTable = (bool)(typeof(Inventory).GetProperty($"CustomText{i}ShowInTable")?.GetValue(inventory) ?? false),
                        SlotNumber = i
                    });
                }

                // Number fields
                if ((bool)(typeof(Inventory).GetProperty($"CustomNumber{i}Enabled")?.GetValue(inventory) ?? false))
                {
                    fields.Add(new CustomFieldDto
                    {
                        FieldType = "number",
                        Name = typeof(Inventory).GetProperty($"CustomNumber{i}Name")?.GetValue(inventory) as string ?? "",
                        Description = typeof(Inventory).GetProperty($"CustomNumber{i}Description")?.GetValue(inventory) as string,
                        ShowInTable = (bool)(typeof(Inventory).GetProperty($"CustomNumber{i}ShowInTable")?.GetValue(inventory) ?? false),
                        SlotNumber = i
                    });
                }

                // Link fields
                if ((bool)(typeof(Inventory).GetProperty($"CustomLink{i}Enabled")?.GetValue(inventory) ?? false))
                {
                    fields.Add(new CustomFieldDto
                    {
                        FieldType = "link",
                        Name = typeof(Inventory).GetProperty($"CustomLink{i}Name")?.GetValue(inventory) as string ?? "",
                        Description = typeof(Inventory).GetProperty($"CustomLink{i}Description")?.GetValue(inventory) as string,
                        ShowInTable = (bool)(typeof(Inventory).GetProperty($"CustomLink{i}ShowInTable")?.GetValue(inventory) ?? false),
                        SlotNumber = i
                    });
                }

                // Bool fields
                if ((bool)(typeof(Inventory).GetProperty($"CustomBool{i}Enabled")?.GetValue(inventory) ?? false))
                {
                    fields.Add(new CustomFieldDto
                    {
                        FieldType = "bool",
                        Name = typeof(Inventory).GetProperty($"CustomBool{i}Name")?.GetValue(inventory) as string ?? "",
                        Description = typeof(Inventory).GetProperty($"CustomBool{i}Description")?.GetValue(inventory) as string,
                        ShowInTable = (bool)(typeof(Inventory).GetProperty($"CustomBool{i}ShowInTable")?.GetValue(inventory) ?? false),
                        SlotNumber = i
                    });
                }
            }

            return fields;
        }

        private static InventoryListItemDto MapToListItem(Inventory i)
        {
            return new InventoryListItemDto
            {
                Id = i.Id,
                Title = i.Title ?? "",
                Description = !string.IsNullOrEmpty(i.Description) && i.Description.Length > 150
                    ? i.Description.Substring(0, 150) + "..."
                    : i.Description ?? "",
                ImageUrl = i.ImageUrl,
                CreatorUsername = i.Creator?.Username ?? "Unknown",
                CategoryName = i.Category?.Name,
                Tags = i.InventoryTags?.Select(it => it.Tag?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>(),
                ItemCount = i.Items?.Count ?? 0,
                CreatedAt = i.CreatedAt
            };
        }

        public async Task<(bool Success, string Message)> SaveCustomIdConfigurationAsync(int inventoryId, List<CustomIdElementDto> elements, int userId)
        {
            try
            {
                var inventory = await _context.Inventories.FindAsync(inventoryId);
                if (inventory == null)
                    return (false, "Inventory not found");

                // Check permissions
                if (inventory.CreatorId != userId)
                {
                    var isAdmin = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin");
                    if (!isAdmin)
                        return (false, "You don't have permission to edit this inventory");
                }

                // Remove existing custom ID elements
                var existingElements = await _context.CustomIdElements
                    .Where(e => e.InventoryId == inventoryId)
                    .ToListAsync();
                _context.CustomIdElements.RemoveRange(existingElements);

                // Add new custom ID elements
                foreach (var element in elements)
                {
                    _context.CustomIdElements.Add(new CustomIdElement
                    {
                        InventoryId = inventoryId,
                        ElementType = element.ElementType,
                        Value = element.Value,
                        Order = element.Order
                    });
                }

                await _context.SaveChangesAsync();
                return (true, "Custom ID configuration saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving custom ID configuration");
                return (false, "Error saving custom ID configuration");
            }
        }

        public async Task<List<CustomIdElementDto>> GetCustomIdConfigurationAsync(int inventoryId)
        {
            var elements = await _context.CustomIdElements
                .Where(e => e.InventoryId == inventoryId)
                .OrderBy(e => e.Order)
                .ToListAsync();

            return elements.Select(e => new CustomIdElementDto
            {
                ElementType = e.ElementType,
                Value = e.Value,
                Order = e.Order
            }).ToList();
        }

        public async Task<List<TagDto>> GetAllTagsAsync()
        {
            return await _context.Tags
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Count = t.InventoryTags.Count
                })
                .Where(t => t.Count > 0)  // Only show tags that are used
                .OrderByDescending(t => t.Count)
                .ToListAsync();
        }

        public async Task<List<InventoryListItemDto>> GetInventoriesByTagAsync(string tagName)
        {
            return await _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
                .Include(i => i.Items)
                .Where(i => i.InventoryTags.Any(it => it.Tag.Name == tagName))
                .Select(i => MapToListItem(i))
                .ToListAsync();
        }
    }
}