using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.DTOs.Api;
using NewLook.Models.Entities;
using NewLook.Services.Interfaces;
using System.Security.Cryptography;

namespace NewLook.Services;

public class InventoryApiTokenService : IInventoryApiTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InventoryApiTokenService> _logger;

    public InventoryApiTokenService(
        ApplicationDbContext context,
        ILogger<InventoryApiTokenService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<InventoryApiTokenDto> GenerateTokenAsync(int inventoryId, int userId, CreateApiTokenDto dto)
    {
        var inventory = await _context.Inventories.FindAsync(inventoryId);
        if (inventory == null)
        {
            throw new Exception("Inventory not found");
        }

        if (inventory.CreatorId != userId)
        {
            var hasAccess = await _context.InventoryAccesses
                .AnyAsync(a => a.InventoryId == inventoryId && a.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You don't have permission to generate tokens for this inventory");
            }
        }

        var token = GenerateSecureToken();
        var expiresAt = dto.ExpiresInDays.HasValue
            ? DateTime.UtcNow.AddDays(dto.ExpiresInDays.Value)
            : (DateTime?)null;

        var apiToken = new InventoryApiToken
        {
            InventoryId = inventoryId,
            Token = token,
            Name = dto.Name,
            ExpiresAt = expiresAt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryApiTokens.Add(apiToken);
        await _context.SaveChangesAsync();

        return MapToDto(apiToken);
    }

    public async Task<List<InventoryApiTokenDto>> GetInventoryTokensAsync(int inventoryId, int userId)
    {
        var inventory = await _context.Inventories.FindAsync(inventoryId);
        if (inventory == null)
        {
            throw new Exception("Inventory not found");
        }

        if (inventory.CreatorId != userId)
        {
            var hasAccess = await _context.InventoryAccesses
                .AnyAsync(a => a.InventoryId == inventoryId && a.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You don't have permission to view tokens for this inventory");
            }
        }

        var tokens = await _context.InventoryApiTokens
            .Where(t => t.InventoryId == inventoryId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tokens.Select(MapToDto).ToList();
    }

    public async Task<bool> RevokeTokenAsync(int tokenId, int userId)
    {
        var token = await _context.InventoryApiTokens
            .Include(t => t.Inventory)
            .FirstOrDefaultAsync(t => t.Id == tokenId);

        if (token == null)
        {
            return false;
        }

        if (token.Inventory.CreatorId != userId)
        {
            var hasAccess = await _context.InventoryAccesses
                .AnyAsync(a => a.InventoryId == token.InventoryId && a.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You don't have permission to revoke this token");
            }
        }

        token.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int?> ValidateTokenAsync(string token)
    {
        var apiToken = await _context.InventoryApiTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive);

        if (apiToken == null)
        {
            return null;
        }

        if (apiToken.ExpiresAt.HasValue && apiToken.ExpiresAt.Value < DateTime.UtcNow)
        {
            return null;
        }

        return apiToken.InventoryId;
    }

    public async Task UpdateLastUsedAsync(string token)
    {
        var apiToken = await _context.InventoryApiTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (apiToken != null)
        {
            apiToken.LastUsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private InventoryApiTokenDto MapToDto(InventoryApiToken token)
    {
        return new InventoryApiTokenDto
        {
            Id = token.Id,
            Token = token.Token,
            Name = token.Name,
            CreatedAt = token.CreatedAt,
            ExpiresAt = token.ExpiresAt,
            IsActive = token.IsActive,
            LastUsedAt = token.LastUsedAt
        };
    }
}
