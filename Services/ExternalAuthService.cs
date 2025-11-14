using NewLook.Models.DTOs.Auth;
using NewLook.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NewLook.Services
{

    public class ExternalAuthService : IExternalAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalAuthService> _logger;
        private readonly IConfiguration _configuration;

        public ExternalAuthService(IHttpClientFactory httpClientFactory, ILogger<ExternalAuthService> logger, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, GoogleUserInfoDto? UserInfo)> ValidateGoogleTokenAsync(string accessToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Google token validation failed: {Error}", error);
                    return (false, "Invalid Google token", null);
                }

                var content = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<GoogleUserInfoDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (userInfo == null || string.IsNullOrEmpty(userInfo.Email))
                {
                    return (false, "Could not retrieve user information from Google", null);
                }

                return (true, "Success", userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google token");
                return (false, "Error validating Google token", null);
            }
        }

        public async Task<(bool Success, string Message, GitHubUserInfoDto? UserInfo)> ValidateGitHubTokenAsync(string accessToken)
        {
            try
            {
                // Get user info
                var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
                userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                userRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("NewLookApp", "1.0"));

                var userResponse = await _httpClient.SendAsync(userRequest);

                if (!userResponse.IsSuccessStatusCode)
                {
                    var error = await userResponse.Content.ReadAsStringAsync();
                    _logger.LogWarning("GitHub token validation failed: {Error}", error);
                    return (false, "Invalid GitHub token", null);
                }

                var userContent = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<GitHubUserInfoDto>(userContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (userInfo == null)
                {
                    return (false, "Could not retrieve user information from GitHub", null);
                }

                // If email is not public, get it from emails endpoint
                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                    emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    emailRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("NewLookApp", "1.0"));

                    var emailResponse = await _httpClient.SendAsync(emailRequest);

                    if (emailResponse.IsSuccessStatusCode)
                    {
                        var emailContent = await emailResponse.Content.ReadAsStringAsync();
                        var emails = JsonSerializer.Deserialize<List<GitHubEmailDto>>(emailContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        var primaryEmail = emails?.FirstOrDefault(e => e.Primary && e.Verified);
                        if (primaryEmail != null)
                        {
                            userInfo.Email = primaryEmail.Email;
                        }
                    }
                }

                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    return (false, "Could not retrieve email from GitHub. Please make sure your email is public or verified.", null);
                }

                return (true, "Success", userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating GitHub token");
                return (false, "Error validating GitHub token", null);
            }
        }

        public async Task<(bool Success, string Message, string? AccessToken)> ExchangeGitHubCodeAsync(string code)
        {
            try
            {
                var clientId = _configuration["Authentication:Github:ClientID"]!;
                var clientSecret = _configuration["Authentication:Github:ClientSecret"]!;
                var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5070";
                var redirectUri = $"{baseUrl}/oauth-callback";

                var requestBody = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "code", code },
            { "redirect_uri", redirectUri }
        };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token")
                {
                    Content = new FormUrlEncodedContent(requestBody)
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                _logger.LogInformation("Exchanging GitHub code: {Body}", JsonSerializer.Serialize(requestBody));

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GitHub code exchange failed: {Error}", responseContent);
                    return (false, "Failed to exchange GitHub code", null);
                }

                var tokenResponse = JsonSerializer.Deserialize<GitHubTokenResponseDto>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return (false, "Invalid response from GitHub", null);
                }

                return (true, "Success", tokenResponse.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exchanging GitHub code");
                return (false, "Error exchanging GitHub code", null);
            }
        }
    }
}