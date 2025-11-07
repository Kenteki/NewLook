using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLook.Models.DTOs.Auth;
using NewLook.Services;
using NewLook.Services.Interfaces;
using System.Security.Claims;

namespace NewLook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IExternalAuthService _externalAuthService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IExternalAuthService externalAuthService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _externalAuthService = externalAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user with email and password
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, response) = await _authService.RegisterAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(response);
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, response) = await _authService.LoginAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(response);
        }

        /// <summary>
        /// Login with Google OAuth
        /// POST /api/auth/google
        /// Body: { "accessToken": "google-access-token" }
        /// </summary>
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] ExternalAuthRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.AccessToken))
                return BadRequest(new { message = "Access token is required" });

            // Validate Google token
            var (success, message, userInfo) = await _externalAuthService.ValidateGoogleTokenAsync(dto.AccessToken);

            if (!success || userInfo == null)
                return BadRequest(new { message });

            // Login or create user
            var username = userInfo.Name ?? userInfo.Email.Split('@')[0];
            var (authSuccess, authMessage, authResponse) = await _authService.ExternalLoginAsync(
                "Google",
                userInfo.GoogleID,
                userInfo.Email,
                username
            );

            if (!authSuccess)
                return BadRequest(new { message = authMessage });

            return Ok(authResponse);
        }

        /// <summary>
        /// Exchange GitHub authorization code for access token
        /// POST /api/auth/github/exchange
        /// Body: { "code": "github-authorization-code" }
        /// </summary>
        [HttpPost("github/exchange")]
        public async Task<IActionResult> ExchangeGitHubCode([FromBody] GitHubCodeExchangeDto dto)
        {
            if (string.IsNullOrEmpty(dto.Code))
                return BadRequest(new { message = "Authorization code is required" });

            var (success, message, accessToken) = await _externalAuthService.ExchangeGitHubCodeAsync(dto.Code);

            if (!success || string.IsNullOrEmpty(accessToken))
                return BadRequest(new { message });

            return Ok(new { accessToken });
        }

        /// <summary>
        /// Login with GitHub OAuth
        /// POST /api/auth/github
        /// Body: { "accessToken": "github-access-token" }
        /// </summary>
        [HttpPost("github")]
        public async Task<IActionResult> GitHubLogin([FromBody] ExternalAuthRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.AccessToken))
                return BadRequest(new { message = "Access token is required" });

            // Validate GitHub token
            var (success, message, userInfo) = await _externalAuthService.ValidateGitHubTokenAsync(dto.AccessToken);

            if (!success || userInfo == null)
                return BadRequest(new { message });

            // Login or create user
            var username = userInfo.Login ?? userInfo.Name ?? userInfo.Email.Split('@')[0];
            var (authSuccess, authMessage, authResponse) = await _authService.ExternalLoginAsync(
                "GitHub",
                userInfo.Id.ToString(),
                userInfo.Email,
                username
            );

            if (!authSuccess)
                return BadRequest(new { message = authMessage });

            return Ok(authResponse);
        }

        /// <summary>
        /// Get current user info
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Roles = roles,
                UI_Language= user.UI_Language,
                UI_Theme = user.UI_Theme
            });
        }

        /// <summary>
        /// Change password for email/password users
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var (success, message) = await _authService.ChangePasswordAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        /// <summary>
        /// Logout (client-side token removal, optional endpoint for tracking)
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // With JWT, logout is handled client-side by removing the token
            // This endpoint can be used for logging or cleanup if needed
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Verify email address
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var (success, message) = await _authService.VerifyEmailAsync(dto.Token);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }

    public class VerifyEmailDto
    {
        public string Token { get; set; } = string.Empty;
    }
}