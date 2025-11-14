using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.DTOs;
using NewLook.Models.Entities;
using NewLook.Services.Interfaces;

namespace NewLook.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommentDto>> GetCommentsByInventoryIdAsync(int inventoryId)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.InventoryId == inventoryId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    InventoryId = c.InventoryId,
                    AuthorId = c.AuthorId,
                    AuthorUsername = c.Author.Username,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, CommentDto? Comment)> AddCommentAsync(CreateCommentDto dto, int userId)
        {
            try
            {
                // Validate inventory exists
                var inventoryExists = await _context.Inventories.AnyAsync(i => i.Id == dto.InventoryId);
                if (!inventoryExists)
                {
                    return (false, "Inventory not found.", null);
                }

                // Validate content
                if (string.IsNullOrWhiteSpace(dto.Content))
                {
                    return (false, "Comment content cannot be empty.", null);
                }

                var comment = new Comment
                {
                    InventoryId = dto.InventoryId,
                    AuthorId = userId,
                    Content = dto.Content.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                // Reload with author info
                var createdComment = await _context.Comments
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync(c => c.Id == comment.Id);

                if (createdComment == null)
                {
                    return (false, "Failed to retrieve created comment.", null);
                }

                var commentDto = new CommentDto
                {
                    Id = createdComment.Id,
                    InventoryId = createdComment.InventoryId,
                    AuthorId = createdComment.AuthorId,
                    AuthorUsername = createdComment.Author.Username,
                    Content = createdComment.Content,
                    CreatedAt = createdComment.CreatedAt
                };

                return (true, "Comment added successfully.", commentDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error adding comment: {ex.Message}", null);
            }
        }
    }
}
