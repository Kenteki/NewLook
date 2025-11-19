using Microsoft.EntityFrameworkCore;
using NewLook.Models.Entities;

namespace NewLook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Inventory> Inventories { get; set; } = null!;
        public DbSet<InventoryAccess> InventoryAccesses { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<InventoryTag> InventoryTags { get; set; } = null!;
        public DbSet<CustomIdElement> CustomIdElements { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<InventoryApiToken> InventoryApiTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
