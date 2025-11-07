using NewLook.Models.DTOs.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace NewLook.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthStateService _authState;
        private readonly IConfiguration _configuration;

        public ApiService(IHttpClientFactory httpClientFactory, AuthStateService authState, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _authState = authState;
            _configuration = configuration;

            var baseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5070";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            var token = await _authState.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            return _httpClient;
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> RegisterAsync(RegisterRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", dto);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    return (true, "Registration successful", data);
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return (false, error?.Message ?? "Registration failed", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", dto);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    return (true, "Login successful", data);
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return (false, error?.Message ?? "Login failed", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, UserDto? Data)> GetCurrentUserAsync()
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.GetAsync("/api/auth/me");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<UserDto>();
                    return (true, "Success", data);
                }
                else
                {
                    return (false, "Failed to get user info", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}