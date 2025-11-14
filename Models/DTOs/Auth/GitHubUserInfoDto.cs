namespace NewLook.Models.DTOs.Auth
{
    public class GitHubUserInfoDto
    {
        public long Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AvatarURL {  get; set; } = string.Empty;
    }
}
