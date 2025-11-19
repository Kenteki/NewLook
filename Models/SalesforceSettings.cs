namespace NewLook.Models;

public class SalesforceSettings
{
    public string InstanceUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SecurityToken { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "v59.0";
    public string TokenEndpoint { get; set; } = "https://login.salesforce.com/services/oauth2/token";
}
