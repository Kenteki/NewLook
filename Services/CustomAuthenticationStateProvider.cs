using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewLook.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        private const string USER_SESSION_KEY = "user_session";

        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(
            ProtectedSessionStorage sessionStorage,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _sessionStorage = sessionStorage;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Try to load user session from protected storage
                var result = await _sessionStorage.GetAsync<UserSession>(USER_SESSION_KEY);

                if (result.Success && result.Value != null)
                {
                    var userSession = result.Value;
                    _currentUser = CreateClaimsPrincipal(userSession);
                    _logger.LogInformation("Loaded user session from storage: {Username}", userSession.Username);
                }
                else
                {
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch (InvalidOperationException)
            {
                // This happens during prerendering when JavaScript interop isn't available yet
                // It's expected and harmless - just return unauthenticated state
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unexpected error loading authentication state");
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            try
            {
                if (userSession == null)
                {
                    // User logged out
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                    await _sessionStorage.DeleteAsync(USER_SESSION_KEY);
                    _logger.LogInformation("User logged out, session cleared");
                }
                else
                {
                    // User logged in - save to protected storage
                    await _sessionStorage.SetAsync(USER_SESSION_KEY, userSession);
                    _currentUser = CreateClaimsPrincipal(userSession);
                    _logger.LogInformation("User session saved: {Username}", userSession.Username);
                }

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating authentication state");
            }
        }

        private ClaimsPrincipal CreateClaimsPrincipal(UserSession userSession)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userSession.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString())
            };

            if (!string.IsNullOrEmpty(userSession.Username))
            {
                claims.Add(new Claim(ClaimTypes.Name, userSession.Username));
            }

            if (!string.IsNullOrEmpty(userSession.FullName))
            {
                claims.Add(new Claim("fullname", userSession.FullName));
            }

            if (!string.IsNullOrEmpty(userSession.AvatarUrl))
            {
                claims.Add(new Claim("avatar", userSession.AvatarUrl));
            }

            var identity = new ClaimsIdentity(claims, "apiauth");
            return new ClaimsPrincipal(identity);
        }

        public void SetUser(UserSession userSession)
        {
            UpdateAuthenticationState(userSession).Wait();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.Identity?.IsAuthenticated ?? false;
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<UserSession>(USER_SESSION_KEY);
                return result.Success ? result.Value?.Token : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            await UpdateAuthenticationState(null);
        }
    }
}
