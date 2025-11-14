namespace NewLook.Models.Entities
{
    public class InventoryAccess
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    }
}