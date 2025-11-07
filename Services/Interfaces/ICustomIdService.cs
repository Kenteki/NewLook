namespace NewLook.Services.Interfaces
{
    public interface ICustomIdService
    {
        Task<string> GenerateCustomIdAsync(int inventoryId);
        Task<string> PreviewCustomIdAsync(List<CustomIdElementDto> elements);
        Task<bool> ValidateCustomIdFormatAsync(int inventoryId, string customId);
    }
}
