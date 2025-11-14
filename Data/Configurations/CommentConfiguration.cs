using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasIndex(c => c.InventoryId);
            builder.HasIndex(c => c.CreatedAt);

            builder.HasOne(c => c.Inventory)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
