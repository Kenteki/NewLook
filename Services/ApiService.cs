using Microsoft.AspNetCore.Components.Authorization;
using NewLook.Models.DTOs.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace NewLook.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiService> _logger;
        private readonly string _baseUrl;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authStateProvider,
            IConfiguration configuration,
            ILogger<ApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider;
            _configuration = configuration;
            _logger = logger;
            _baseUrl = Environment.GetEnvironmentVariable("APP_BASE_URL")
                ?? _configuration["BaseUrl"]
                ?? "http://localhost:5070";
        }

        private async Task<HttpClient> CreateHttpClientAsync(bool authenticated = false)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseUrl);

            if (authenticated && _authStateProvider is CustomAuthenticationStateProvider customProvider)
            {
                var token = await customProvider.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> RegisterAsync(RegisterRequestDto dto)
        {
            try
            {
                var client = await CreateHttpClientAsync();
                var response = await client.PostAsJsonAsync("/api/auth/register", dto);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    return (true, "Registration successful", data);
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.Message ?? "Registration failed", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginRequestDto dto)
        {
            try
            {
                var client = await CreateHttpClientAsync();
                var response = await client.PostAsJsonAsync("/api/auth/login", dto);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    return (true, "Login successful", data);
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.Message ?? "Login failed", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, string? AccessToken)> ExchangeGoogleCodeAsync(string code)
        {
            try
            {
                var clientId = _configuration["Authentication:Google:ClientID"];
                var clientSecret = _configuration["Authentication:Google:ClientSecret"];
                var baseUrl = Environment.GetEnvironmentVariable("APP_BASE_URL")
                    ?? _configuration["BaseUrl"]
                    ?? "http://localhost:5070";
                var redirectUri = $"{baseUrl}/oauth-callback";

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    _logger.LogError("Google OAuth credentials not configured");
                    return (false, "Google OAuth not configured", null);
                }

                var parameters = new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                };

                var content = new FormUrlEncodedContent(parameters);

                _logger.LogInformation("Exchanging Google code with redirect URI: {RedirectUri}", redirectUri);

                using var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Google response status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Google response: {Response}", responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google code exchange failed: {Error}", responseContent);
                    return (false, $"Failed to exchange Google code: {responseContent}", null);
                }

                var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponseDto>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    _logger.LogError("Failed to deserialize Google token response");
                    return (false, "Invalid response from Google", null);
                }

                _logger.LogInformation("Successfully exchanged Google code for access token");
                return (true, "Success", tokenResponse.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exchanging Google code");
                return (false, $"Error exchanging Google code: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, string? AccessToken)> ExchangeGitHubCodeAsync(string code)
        {
            try
            {
                _logger.LogInformation("Exchanging GitHub code");
                var client = await CreateHttpClientAsync();
                var response = await client.PostAsJsonAsync("/api/auth/github/exchange", new { Code = code });

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("GitHub exchange response status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("GitHub exchange response: {Response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<TokenExchangeResponse>();
                    if (data != null && !string.IsNullOrEmpty(data.AccessToken))
                    {
                        _logger.LogInformation("Successfully exchanged GitHub code for access token");
                        return (true, "Code exchanged successfully", data.AccessToken);
                    }
                    else
                    {
                        _logger.LogError("GitHub token response was null or empty");
                        return (false, "Invalid response from GitHub", null);
                    }
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.Message ?? $"Failed to exchange GitHub code: {responseContent}", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub code exchange failed");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> AuthenticateWithGoogleAsync(string accessToken)
        {
            try
            {
                var client = await CreateHttpClientAsync();
                var response = await client.PostAsJsonAsync("/api/auth/google", new { AccessToken = accessToken });

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    return (true, "Google authentication successful", data);
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.Message ?? "Google authentication failed", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google authentication failed");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> AuthenticateWithGitHubAsync(string accessToken)
        {
            try
            {
                var client = await CreateHttpClientAsync();
                var response = await client.PostAsJsonAsync("/api/auth/github", new { AccessToken = accessToken });

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    return (true, "GitHub authentication successful", data);
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.Message ?? "GitHub authentication failed", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub authentication failed");
                return (false, $"Error: {ex.Message}", null);
            }
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class TokenExchangeResponse
    {
        public string AccessToken { get; set; } = string.Empty;
    }
}
