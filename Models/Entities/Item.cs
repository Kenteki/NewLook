namespace NewLook.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;

        public string CustomId { get; set; } = string.Empty; // Generation based on (Inventory customID format)

        // Custom field values based on inventory configuration
        public string? CustomString1Value { get; set; }
        public string? CustomString2Value { get; set; }
        public string? CustomString3Value { get; set; }

        public string? CustomText1Value { get; set; }
        public string? CustomText2Value { get; set; }
        public string? CustomText3Value { get; set; }

        public decimal? CustomNumber1Value { get; set; }
        public decimal? CustomNumber2Value { get; set; }
        public decimal? CustomNumber3Value { get; set; }

        public string? CustomLink1Value { get; set; }
        public string? CustomLink2Value { get; set; }
        public string? CustomLink3Value { get; set; }

        public bool? CustomBool1Value { get; set; }
        public bool? CustomBool2Value { get; set; }
        public bool? CustomBool3Value { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Version lock
        public int Version { get; set; } = 1;
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
