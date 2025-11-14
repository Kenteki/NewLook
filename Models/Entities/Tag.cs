namespace NewLook.Models.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
    }
}
