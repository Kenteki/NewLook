using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLook.Models.DTOs.Item;
using NewLook.Services;
using NewLook.Services.Interfaces;
using System.Security.Claims;

namespace NewLook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet("inventory/{inventoryId}")]
        public async Task<IActionResult> GetByInventoryId(int inventoryId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedId))
                userId = parsedId;

            var items = await _itemService.GetItemsByInventoryIdAsync(inventoryId, userId);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedId))
                userId = parsedId;

            var item = await _itemService.GetItemByIdAsync(id, userId);
            if (item is null)
                return NotFound();

            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateItemDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message, data) = await _itemService.CreateItemAsync(dto, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(data);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateItemDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message, data) = await _itemService.UpdateItemAsync(id, dto, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(data);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message) = await _itemService.DeleteItemAsync(id, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize]
        [HttpPost("{id}/like")]
        public async Task<IActionResult> Like(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message) = await _itemService.LikeItemAsync(id, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize]
        [HttpDelete("{id}/like")]
        public async Task<IActionResult> Unlike(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message) = await _itemService.UnlikeItemAsync(id, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}