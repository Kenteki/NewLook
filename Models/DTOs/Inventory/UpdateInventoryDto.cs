namespace NewLook.Models.DTOs.Inventory
{
    public class UpdateInventoryDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public bool IsPublic { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<CustomFieldDto> CustomFields { get; set; } = new();
        public int Version { get; set; } // Version control
    }
}