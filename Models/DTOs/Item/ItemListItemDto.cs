namespace NewLook.Models.DTOs.Item
{
    public class ItemListItemDto
    {
        public int Id { get; set; }
        public string CustomId { get; set; } = string.Empty;
        
        // Public fields
        public Dictionary<string, object?> DisplayFields { get; set; } = new();
        
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
    }
}