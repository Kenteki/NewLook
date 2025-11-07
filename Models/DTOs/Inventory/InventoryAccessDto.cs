namespace NewLook.Models.DTOs.Inventory
{
    public class InventoryAccessDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime GrantedAt { get; set; }
    }

    public class GrantAccessDto
    {
        public string EmailOrUsername { get; set; } = string.Empty;
    }
}