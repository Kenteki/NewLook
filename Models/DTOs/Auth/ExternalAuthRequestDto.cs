namespace NewLook.Models.DTOs.Auth
{
    public class ExternalAuthRequestDto
    {
        public string Provider { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
