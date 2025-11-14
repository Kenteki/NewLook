using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Provider)
                .HasMaxLength(50);

            builder.Property(u => u.ProviderID)
                .HasMaxLength(256);

            builder.Property(u => u.UI_Language)
                .HasMaxLength(10)
                .HasDefaultValue("en");

            builder.Property(u => u.UI_Theme)
                .HasMaxLength(20)
                .HasDefaultValue("light");

            // Indexes
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.Username);
            builder.HasIndex(u => new { u.Provider, u.ProviderID })
                .IsUnique()
                .HasFilter("\"Provider\" IS NOT NULL AND \"ProviderID\" IS NOT NULL");

            // Relationships
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.OwnedInventories)
                .WithOne(i => i.Creator)
                .HasForeignKey(i => i.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.InventoryAccesses)
                .WithOne(ia => ia.User)
                .HasForeignKey(ia => ia.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.CreatedItems)
                .WithOne(i => i.CreatedBy)
                .HasForeignKey(i => i.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Comments)
                .WithOne(c => c.Author)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Likes)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
