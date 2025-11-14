using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Description)
                .HasMaxLength(2000);

            builder.Property(i => i.ImageUrl)
                .HasMaxLength(500);

            // Custom field names String, Multiple, Number, Bool
            builder.Property(i => i.CustomString1Name).HasMaxLength(100);
            builder.Property(i => i.CustomString2Name).HasMaxLength(100);
            builder.Property(i => i.CustomString3Name).HasMaxLength(100);

            builder.Property(i => i.CustomText1Name).HasMaxLength(100);
            builder.Property(i => i.CustomText2Name).HasMaxLength(100);
            builder.Property(i => i.CustomText3Name).HasMaxLength(100);

            builder.Property(i => i.CustomNumber1Name).HasMaxLength(100);
            builder.Property(i => i.CustomNumber2Name).HasMaxLength(100);
            builder.Property(i => i.CustomNumber3Name).HasMaxLength(100);

            builder.Property(i => i.CustomLink1Name).HasMaxLength(100);
            builder.Property(i => i.CustomLink2Name).HasMaxLength(100);
            builder.Property(i => i.CustomLink3Name).HasMaxLength(100);

            builder.Property(i => i.CustomBool1Name).HasMaxLength(100);
            builder.Property(i => i.CustomBool2Name).HasMaxLength(100);
            builder.Property(i => i.CustomBool3Name).HasMaxLength(100);

            // Custom field descriptions
            builder.Property(i => i.CustomString1Description).HasMaxLength(500);
            builder.Property(i => i.CustomString2Description).HasMaxLength(500);
            builder.Property(i => i.CustomString3Description).HasMaxLength(500);

            builder.Property(i => i.CustomText1Description).HasMaxLength(500);
            builder.Property(i => i.CustomText2Description).HasMaxLength(500);
            builder.Property(i => i.CustomText3Description).HasMaxLength(500);

            builder.Property(i => i.CustomNumber1Description).HasMaxLength(500);
            builder.Property(i => i.CustomNumber2Description).HasMaxLength(500);
            builder.Property(i => i.CustomNumber3Description).HasMaxLength(500);

            builder.Property(i => i.CustomLink1Description).HasMaxLength(500);
            builder.Property(i => i.CustomLink2Description).HasMaxLength(500);
            builder.Property(i => i.CustomLink3Description).HasMaxLength(500);

            builder.Property(i => i.CustomBool1Description).HasMaxLength(500);
            builder.Property(i => i.CustomBool2Description).HasMaxLength(500);
            builder.Property(i => i.CustomBool3Description).HasMaxLength(500);

            // Index
            builder.HasIndex(i => i.CreatorId);
            builder.HasIndex(i => i.CategoryId);
            builder.HasIndex(i => i.IsPublic);
            builder.HasIndex(i => i.CreatedAt);

            // Version lock
            builder.Property(i => i.Version)
                .IsConcurrencyToken();

            // Relationships
            builder.HasOne(i => i.Creator)
                .WithMany(u => u.OwnedInventories)
                .HasForeignKey(i => i.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Category)
                .WithMany(c => c.Inventories)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(i => i.InventoryAccesses)
                .WithOne(ia => ia.Inventory)
                .HasForeignKey(ia => ia.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.Items)
                .WithOne(item => item.Inventory)
                .HasForeignKey(item => item.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.InventoryTags)
                .WithOne(it => it.Inventory)
                .HasForeignKey(it => it.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.Comments)
                .WithOne(c => c.Inventory)
                .HasForeignKey(c => c.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.CustomIdElements)
                .WithOne(ce => ce.Inventory)
                .HasForeignKey(ce => ce.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
