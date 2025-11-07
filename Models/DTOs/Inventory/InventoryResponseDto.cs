namespace NewLook.Models.DTOs.Inventory
{
    public class InventoryResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CreatorId { get; set; }
        public string CreatorUsername { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsPublic { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<CustomFieldDto> CustomFields { get; set; } = new();
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Version { get; set; } // Version control
        public bool HasWriteAccess { get; set; }
    }
}