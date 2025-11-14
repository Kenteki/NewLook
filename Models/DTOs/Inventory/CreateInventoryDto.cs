using System.ComponentModel.DataAnnotations;

namespace NewLook.Models.DTOs.Inventory
{
    public class CreateInventoryDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        
        public int? CategoryId { get; set; }
        
        public bool IsPublic { get; set; } = false;
        
        public List<string> Tags { get; set; } = new();
        
        // Custom fields configuration
        public List<CustomFieldDto> CustomFields { get; set; } = new();
    }

    public class CustomFieldDto
    {
        public string FieldType { get; set; } = string.Empty; // "string", "text", "number", "link", "bool"
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool ShowInTable { get; set; } = false;
    }
}