namespace NewLook.Models.DTOs.Auth
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string UI_Language { get; set; } = "en";
        public string UI_Theme { get; set; } = "light";
    }
}
