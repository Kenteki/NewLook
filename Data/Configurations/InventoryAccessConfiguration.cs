using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class InventoryAccessConfiguration : IEntityTypeConfiguration<InventoryAccess>
    {
        public void Configure(EntityTypeBuilder<InventoryAccess> builder)
        {
            builder.HasKey(ia => ia.Id);

            // Unique constraint, allows one user per one inventory
            builder.HasIndex(ia => new { ia.InventoryId, ia.UserId }).IsUnique();

            builder.HasOne(ia => ia.Inventory)
                .WithMany(i => i.InventoryAccesses)
                .HasForeignKey(ia => ia.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ia => ia.User)
                .WithMany(u => u.InventoryAccesses)
                .HasForeignKey(ia => ia.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
