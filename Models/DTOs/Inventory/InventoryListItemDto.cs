namespace NewLook.Models.DTOs.Inventory
{
    public class InventoryListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string CreatorUsername { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public List<string> Tags { get; set; } = new();
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}