namespace NewLook.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        // Authorization
        public string? Provider { get; set; } // Google/Facebook
        public string? ProviderID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }

        // Email Verification
        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        // User description
        public string Username { get; set; } = string.Empty;
        public bool isBlocked { get; set; } = false;
        public string UI_Language { get; set; } = "en";
        public string UI_Theme { get; set; } = "light";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Inventory> OwnedInventories { get; set; } = new List<Inventory>();
        public ICollection<InventoryAccess> InventoryAccesses { get; set; } = new List<InventoryAccess>();
        public ICollection<Item> CreatedItems { get; set; } = new List<Item>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
