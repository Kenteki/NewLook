namespace NewLook.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;
        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
