using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.CustomId)
                .IsRequired()
                .HasMaxLength(200);

            // String fields
            builder.Property(i => i.CustomString1Value).HasMaxLength(500);
            builder.Property(i => i.CustomString2Value).HasMaxLength(500);
            builder.Property(i => i.CustomString3Value).HasMaxLength(500);

            // Multiline fields
            builder.Property(i => i.CustomText1Value).HasMaxLength(5000);
            builder.Property(i => i.CustomText2Value).HasMaxLength(5000);
            builder.Property(i => i.CustomText3Value).HasMaxLength(5000);

            // Numeric fields
            builder.Property(i => i.CustomNumber1Value).HasPrecision(18, 2);
            builder.Property(i => i.CustomNumber2Value).HasPrecision(18, 2);
            builder.Property(i => i.CustomNumber3Value).HasPrecision(18, 2);

            // Link fields
            builder.Property(i => i.CustomLink1Value).HasMaxLength(1000);
            builder.Property(i => i.CustomLink2Value).HasMaxLength(1000);
            builder.Property(i => i.CustomLink3Value).HasMaxLength(1000);

            // Uniq index for CustomID
            builder.HasIndex(i => new { i.InventoryId, i.CustomId }).IsUnique();

            builder.HasIndex(i => i.CreatedById);
            builder.HasIndex(i => i.CreatedAt);

            // Version lock
            builder.Property(i => i.Version).IsConcurrencyToken();

            // Relationships
            builder.HasOne(i => i.Inventory)
                .WithMany(inv => inv.Items)
                .HasForeignKey(i => i.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.CreatedBy)
                .WithMany(u => u.CreatedItems)
                .HasForeignKey(i => i.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Likes)
                .WithOne(l => l.Item)
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
