namespace NewLook.Services.Interfaces;

public interface ISalesforceAuthService
{
    Task<string> GetAccessTokenAsync();
    Task<bool> ValidateConnectionAsync();
    string GetInstanceUrl();
}
