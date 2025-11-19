using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLook.Models.DTOs.Api;
using NewLook.Services.Interfaces;

namespace NewLook.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryApiController : ControllerBase
{
    private readonly IInventoryApiTokenService _tokenService;
    private readonly IInventoryAggregationService _aggregationService;
    private readonly ILogger<InventoryApiController> _logger;

    public InventoryApiController(
        IInventoryApiTokenService tokenService,
        IInventoryAggregationService aggregationService,
        ILogger<InventoryApiController> logger)
    {
        _tokenService = tokenService;
        _aggregationService = aggregationService;
        _logger = logger;
    }

    [HttpPost("{inventoryId}/tokens")]
    [Authorize]
    public async Task<IActionResult> GenerateToken(int inventoryId, [FromBody] CreateApiTokenDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var token = await _tokenService.GenerateTokenAsync(inventoryId, userId, dto);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating API token for inventory {InventoryId}", inventoryId);
            return StatusCode(500, new { message = "An error occurred while generating the token" });
        }
    }

    [HttpGet("{inventoryId}/tokens")]
    [Authorize]
    public async Task<IActionResult> GetTokens(int inventoryId)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var tokens = await _tokenService.GetInventoryTokensAsync(inventoryId, userId);
            return Ok(tokens);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving API tokens for inventory {InventoryId}", inventoryId);
            return StatusCode(500, new { message = "An error occurred while retrieving tokens" });
        }
    }

    [HttpDelete("tokens/{tokenId}")]
    [Authorize]
    public async Task<IActionResult> RevokeToken(int tokenId)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var result = await _tokenService.RevokeTokenAsync(tokenId, userId);
            if (!result)
            {
                return NotFound(new { message = "Token not found" });
            }

            return Ok(new { message = "Token revoked successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking API token {TokenId}", tokenId);
            return StatusCode(500, new { message = "An error occurred while revoking the token" });
        }
    }

    [HttpGet("data")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAggregatedData([FromHeader(Name = "X-API-Token")] string apiToken)
    {
        try
        {
            if (string.IsNullOrEmpty(apiToken))
            {
                return Unauthorized(new { message = "API token is required" });
            }

            var inventoryId = await _tokenService.ValidateTokenAsync(apiToken);
            if (!inventoryId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            await _tokenService.UpdateLastUsedAsync(apiToken);

            var aggregatedData = await _aggregationService.GetAggregatedDataAsync(inventoryId.Value);
            return Ok(aggregatedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aggregated data");
            return StatusCode(500, new { message = "An error occurred while retrieving data" });
        }
    }
}
