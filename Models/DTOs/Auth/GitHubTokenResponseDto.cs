namespace NewLook.Models.DTOs.Auth
{
    public class GitHubTokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
    }
}
