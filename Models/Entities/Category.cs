namespace NewLook.Models.Entities
{
    public class Category
    {
        // used only for sotring and filtering in the tables of inventories.
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
