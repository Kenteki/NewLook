namespace NewLook.Models.Entities
{
    public class CustomIdElement
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;

        public int Order { get; set; } // order for element types

        // Element type: "Fixed", "Random20", "Random32", "Random6", "Random9", "Guid", "DateTime", "Sequence"
        public string ElementType { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    // Element types
    public enum CustomIdElementType
    {
        // Fixed text with Unicode support
        Fixed,
        // 20-bit random number (0-1,048,575)
        Random20,
        // 32-bit random number (0-4,294,967,295)
        Random32,
        // 6-digit random (000000-999999)
        Random6,
        // 9-digit random (000000000-999999999)
        Random9,
        // Full GUID or shortened version
        Guid,
        // Date/time at creation
        DateTime,
        // Auto-incrementing sequence
        Sequence
    }
}
