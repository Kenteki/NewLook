using Microsoft.Extensions.Options;
using NewLook.Models;
using NewLook.Models.DTOs.Salesforce;
using NewLook.Services.Interfaces;
using System.Text.Json;

namespace NewLook.Services;

public class SalesforceAuthService : ISalesforceAuthService
{
    private readonly SalesforceSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SalesforceAuthService> _logger;
    private string? _cachedToken;
    private string? _instanceUrl;
    private DateTime _tokenExpiry;

    public SalesforceAuthService(
        IOptions<SalesforceSettings> settings,
        IHttpClientFactory httpClientFactory,
        ILogger<SalesforceAuthService> logger)
    {
        _settings = settings.Value;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _tokenExpiry = DateTime.MinValue;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _cachedToken;
        }

        var passwordWithToken = _settings.Password + _settings.SecurityToken;

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["username"] = _settings.Username,
            ["password"] = passwordWithToken
        };

        var requestData = new FormUrlEncodedContent(formData);

        try
        {
            var response = await _httpClient.PostAsync(_settings.TokenEndpoint, requestData);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Salesforce authentication failed: {StatusCode} - {Content}",
                    response.StatusCode, content);
                throw new Exception($"Salesforce authentication failed: {content}");
            }

            var tokenResponse = JsonSerializer.Deserialize<SalesforceTokenResponseDto>(content);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new Exception("Invalid token response from Salesforce");
            }

            _cachedToken = tokenResponse.AccessToken;
            _instanceUrl = tokenResponse.InstanceUrl;
            _tokenExpiry = DateTime.UtcNow.AddMinutes(55);

            return _cachedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with Salesforce");
            throw;
        }
    }

    public async Task<bool> ValidateConnectionAsync()
    {
        try
        {
            var token = await GetAccessTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
        catch
        {
            return false;
        }
    }

    public string GetInstanceUrl()
    {
        return _instanceUrl ?? _settings.InstanceUrl;
    }
}
