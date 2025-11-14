namespace NewLook.Models.DTOs.Admin
{
    public class UserManagementDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public int InventoryCount { get; set; }
    }
}
