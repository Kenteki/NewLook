using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.DTOs.Auth;
using NewLook.Models.Entities;
using BCrypt.Net;
using NewLook.Services.Interfaces;

namespace NewLook.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IJwtService jwtService, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Response)> RegisterAsync(RegisterRequestDto dto)
        {
            // Validate
            if (dto.Password != dto.ConfirmPassword)
                return (false, "Passwords do not match", null);

            if (dto.Password.Length < 6)
                return (false, "Password must be at least 6 characters", null);

            // Check if user exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                return (false, "User with this email already exists", null);

            // Generate verification token
            var verificationToken = Guid.NewGuid().ToString();

            // Create user
            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Provider = null,
                ProviderID = null,
                UI_Language = "en",
                UI_Theme = "light",
                CreatedAt = DateTime.UtcNow,
                isBlocked = false,
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            _context.Users.Add(user);

            // Assign default "User" role
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    User = user,
                    RoleId = userRole.Id
                });
            }

            await _context.SaveChangesAsync();

            // Send verification email
            var baseUrl = _configuration["AppBaseUrl"] ?? "http://localhost:5070";
            var verificationLink = $"{baseUrl}/verify-email?token={verificationToken}";
            await _emailService.SendVerificationEmailAsync(user.Email, user.Username, verificationLink);

            // Generate token (user can login but some features may be restricted)
            var roles = new List<string> { "User" };
            var token = _jwtService.GenerateJwtToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = "",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = roles,
                    UI_Language = user.UI_Language,
                    UI_Theme = user.UI_Theme
                }
            };

            return (true, "Registration successful. Please check your email to verify your account.", response);
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Response)> LoginAsync(LoginRequestDto dto)
        {
            // Find user
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return (false, "Invalid email or password", null);

            // Check if user has password (not OAuth user)
            if (string.IsNullOrEmpty(user.PasswordHash))
                return (false, "This account uses social login. Please use Google or GitHub to sign in.", null);

            // Check if blocked
            if (user.isBlocked)
                return (false, "Your account has been blocked", null);

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, "Invalid email or password", null);

            // Get roles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // Generate token
            var token = _jwtService.GenerateJwtToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = "",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = roles,
                    UI_Language = user.UI_Language,
                    UI_Theme = user.UI_Theme
                }
            };

            return (true, "Login successful", response);
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Response)> ExternalLoginAsync(
            string provider, string ProviderID, string email, string username)
        {
            // Find existing user by provider
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Provider == provider && u.ProviderID == ProviderID);

            bool isNewUser = false;

            if (user == null)
            {
                // Check if email already exists
                var existingEmailUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (existingEmailUser != null)
                {
                    return (false, "An account with this email already exists. Please login with email/password.", null);
                }

                // Create new user
                user = new User
                {
                    Email = email,
                    Username = username,
                    Provider = provider,
                    ProviderID = ProviderID,
                    PasswordHash = null,
                    UI_Language = "en",
                    UI_Theme = "light",
                    CreatedAt = DateTime.UtcNow,
                    isBlocked = false,
                    IsEmailVerified = true // OAuth users are already verified
                };

                _context.Users.Add(user);

                // Assign default role
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (userRole != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        User = user,
                        RoleId = userRole.Id
                    });
                }

                await _context.SaveChangesAsync();
                isNewUser = true;
            }

            // Check if blocked
            if (user.isBlocked)
                return (false, "Your account has been blocked", null);

            // Get roles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // Generate token
            var token = _jwtService.GenerateJwtToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = "",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = roles,
                    UI_Language = user.UI_Language,
                    UI_Theme = user.UI_Theme
                }
            };

            var message = isNewUser ? "Account created successfully" : "Login successful";
            return (true, message, response);
        }

        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

            if (user == null)
                return (false, "Invalid verification token");

            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
                return (false, "Verification token has expired");

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            await _context.SaveChangesAsync();

            return (true, "Email verified successfully");
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return (false, "New passwords do not match");

            if (dto.NewPassword.Length < 6)
                return (false, "Password must be at least 6 characters");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found");

            if (string.IsNullOrEmpty(user.PasswordHash))
                return (false, "Cannot change password for social login accounts");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return (false, "Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return (true, "Password changed successfully");
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}