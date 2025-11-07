using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class InventoryTagConfiguration : IEntityTypeConfiguration<InventoryTag>
    {
        public void Configure(EntityTypeBuilder<InventoryTag> builder)
        {
            builder.HasKey(it => new { it.InventoryId, it.TagId });

            builder.HasOne(it => it.Inventory)
                .WithMany(i => i.InventoryTags)
                .HasForeignKey(it => it.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(it => it.Tag)
                .WithMany(t => t.InventoryTags)
                .HasForeignKey(it => it.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
