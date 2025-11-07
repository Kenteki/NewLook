using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class CustomIdElementConfiguration : IEntityTypeConfiguration<CustomIdElement>
    {
        public void Configure(EntityTypeBuilder<CustomIdElement> builder)
        {
            builder.HasKey(ce => ce.Id);

            builder.Property(ce => ce.ElementType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(ce => new { ce.InventoryId, ce.Order });

            builder.HasOne(ce => ce.Inventory)
                .WithMany(i => i.CustomIdElements)
                .HasForeignKey(ce => ce.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
