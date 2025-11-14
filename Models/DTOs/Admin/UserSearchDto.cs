namespace NewLook.Models.DTOs.Admin
{
    public class UserSearchDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
