namespace NewLook.Models.Entities
{
    public class Inventory
    {
        // Inventory properties
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; } = false;

        // Link to creator and category
        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Custom fields configuration
        // String fields
        public bool CustomString1Enabled { get; set; } = false;
        public string? CustomString1Name { get; set; }
        public string? CustomString1Description { get; set; }
        public bool CustomString1ShowInTable { get; set; } = false;

        public bool CustomString2Enabled { get; set; } = false;
        public string? CustomString2Name { get; set; }
        public string? CustomString2Description { get; set; }
        public bool CustomString2ShowInTable { get; set; } = false;

        public bool CustomString3Enabled { get; set; } = false;
        public string? CustomString3Name { get; set; }
        public string? CustomString3Description { get; set; }
        public bool CustomString3ShowInTable { get; set; } = false;

        // Multiline text fields
        public bool CustomText1Enabled { get; set; } = false;
        public string? CustomText1Name { get; set; }
        public string? CustomText1Description { get; set; }
        public bool CustomText1ShowInTable { get; set; } = false;

        public bool CustomText2Enabled { get; set; } = false;
        public string? CustomText2Name { get; set; }
        public string? CustomText2Description { get; set; }
        public bool CustomText2ShowInTable { get; set; } = false;

        public bool CustomText3Enabled { get; set; } = false;
        public string? CustomText3Name { get; set; }
        public string? CustomText3Description { get; set; }
        public bool CustomText3ShowInTable { get; set; } = false;

        // Numeric fields
        public bool CustomNumber1Enabled { get; set; } = false;
        public string? CustomNumber1Name { get; set; }
        public string? CustomNumber1Description { get; set; }
        public bool CustomNumber1ShowInTable { get; set; } = false;

        public bool CustomNumber2Enabled { get; set; } = false;
        public string? CustomNumber2Name { get; set; }
        public string? CustomNumber2Description { get; set; }
        public bool CustomNumber2ShowInTable { get; set; } = false;

        public bool CustomNumber3Enabled { get; set; } = false;
        public string? CustomNumber3Name { get; set; }
        public string? CustomNumber3Description { get; set; }
        public bool CustomNumber3ShowInTable { get; set; } = false;

        // Link/Document fields
        public bool CustomLink1Enabled { get; set; } = false;
        public string? CustomLink1Name { get; set; }
        public string? CustomLink1Description { get; set; }
        public bool CustomLink1ShowInTable { get; set; } = false;

        public bool CustomLink2Enabled { get; set; } = false;
        public string? CustomLink2Name { get; set; }
        public string? CustomLink2Description { get; set; }
        public bool CustomLink2ShowInTable { get; set; } = false;

        public bool CustomLink3Enabled { get; set; } = false;
        public string? CustomLink3Name { get; set; }
        public string? CustomLink3Description { get; set; }
        public bool CustomLink3ShowInTable { get; set; } = false;

        // Boolean fields
        public bool CustomBool1Enabled { get; set; } = false;
        public string? CustomBool1Name { get; set; }
        public string? CustomBool1Description { get; set; }
        public bool CustomBool1ShowInTable { get; set; } = false;

        public bool CustomBool2Enabled { get; set; } = false;
        public string? CustomBool2Name { get; set; }
        public string? CustomBool2Description { get; set; }
        public bool CustomBool2ShowInTable { get; set; } = false;

        public bool CustomBool3Enabled { get; set; } = false;
        public string? CustomBool3Name { get; set; }
        public string? CustomBool3Description { get; set; }
        public bool CustomBool3ShowInTable { get; set; } = false;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Version lock
        public int Version { get; set; } = 1;

        // Relations
        public ICollection<InventoryAccess> InventoryAccesses { get; set; } = new List<InventoryAccess>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<CustomIdElement> CustomIdElements { get; set; } = new List<CustomIdElement>();
    }
}