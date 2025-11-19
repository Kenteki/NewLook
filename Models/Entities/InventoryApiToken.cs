using System.ComponentModel.DataAnnotations;

namespace NewLook.Models.Entities;

public class InventoryApiToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    [Required]
    [StringLength(64)]
    public string Token { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Name { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastUsedAt { get; set; }
}
