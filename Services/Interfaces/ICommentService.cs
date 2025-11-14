using NewLook.Models.DTOs;

namespace NewLook.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetCommentsByInventoryIdAsync(int inventoryId);
        Task<(bool Success, string Message, CommentDto? Comment)> AddCommentAsync(CreateCommentDto dto, int userId);
    }
}
