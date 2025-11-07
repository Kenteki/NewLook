using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLook.Models.Entities;

namespace NewLook.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        // used only for sotring and filtering in the tables of inventories.
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Name).IsUnique();

            
            builder.HasData(
                new Category { Id = 1, Name = "Equipment" },
                new Category { Id = 2, Name = "Devices" },
                new Category { Id = 3, Name = "Books" },
                new Category { Id = 4, Name = "Documents" },
                new Category { Id = 5, Name = "Employees" },
                new Category { Id = 6, Name = "Other" }
            );
        }
    }
}
