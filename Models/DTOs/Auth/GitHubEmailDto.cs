namespace NewLook.Models.DTOs.Auth
{
    public class GitHubEmailDto
    {
        public string Email { get; set; } = string.Empty;
        public bool Primary { get; set; }
        public bool Verified { get; set; }
        public string? Visibility { get; set; }
    }
}
