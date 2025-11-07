using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLook.Models.DTOs.Inventory;
using NewLook.Models.DTOs.Inventory.Interfaces;
using NewLook.Services;
using System.Security.Claims;

namespace NewLook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventories = await _inventoryService.GetLatestInventoriesAsync(50);
            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedId))
                userId = parsedId;

            var inventory = await _inventoryService.GetInventoryByIdAsync(id, userId);
            if (inventory is null)
                return NotFound();

            return Ok(inventory!);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInventoryDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var response = await _inventoryService.CreateInventoryAsync(dto, userId);
            if (!response.Success)
                return BadRequest(new { response.Message });

            return Ok(response.Data);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInventoryDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var response = await _inventoryService.UpdateInventoryAsync(id, dto, userId);
            if (!response.Success)
                return BadRequest(new { response.Message });

            return Ok(response.Data);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message) = await _inventoryService.DeleteInventoryAsync(id, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyInventories()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var inventories = await _inventoryService.GetUserInventoriesAsync(userId);
            return Ok(inventories);
        }

        [Authorize]
        [HttpGet("shared")]
        public async Task<IActionResult> GetSharedInventories()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var inventories = await _inventoryService.GetInventoriesWithAccessAsync(userId);
            return Ok(inventories);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Search query is required" });

            var results = await _inventoryService.SearchInventoriesAsync(q);
            return Ok(results);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular()
        {
            var inventories = await _inventoryService.GetPopularInventoriesAsync(5);
            return Ok(inventories);
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetStats(int id)
        {
            var stats = await _inventoryService.GetInventoryStatsAsync(id);
            if (stats is null)
                return NotFound();

            return Ok(stats!);
        }

        [Authorize]
        [HttpGet("{id}/access")]
        public async Task<IActionResult> GetAccessList(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var accessList = await _inventoryService.GetInventoryAccessListAsync(id, userId);
            return Ok(accessList);
        }

        [Authorize]
        [HttpPost("{id}/access")]
        public async Task<IActionResult> GrantAccess(int id, [FromBody] GrantAccessDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message) = await _inventoryService.GrantAccessAsync(id, dto.EmailOrUsername, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize]
        [HttpDelete("{id}/access/{targetUserId}")]
        public async Task<IActionResult> RevokeAccess(int id, int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message) = await _inventoryService.RevokeAccessAsync(id, targetUserId, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}