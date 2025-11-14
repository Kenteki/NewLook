using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.DTOs.Admin;
using NewLook.Models.Entities;
using NewLook.Services.Interfaces;

namespace NewLook.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserManagementDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var result = new List<UserManagementDto>();
            foreach (var user in users)
            {
                var inventoryCount = await _context.Inventories
                    .CountAsync(i => i.CreatorId == user.Id);

                result.Add(new UserManagementDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    IsBlocked = user.isBlocked,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    CreatedAt = user.CreatedAt,
                    InventoryCount = inventoryCount
                });
            }

            return result;
        }

        public async Task<(bool Success, string Message)> BlockUserAsync(int userId, int adminId)
        {
            try
            {
                // Check if admin
                if (!await IsUserAdminAsync(adminId))
                    return (false, "Only admins can block users");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return (false, "User not found");

                if (user.isBlocked)
                    return (false, "User is already blocked");

                user.isBlocked = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {userId} blocked by admin {adminId}");
                return (true, "User blocked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error blocking user {userId}");
                return (false, "Error blocking user");
            }
        }

        public async Task<(bool Success, string Message)> UnblockUserAsync(int userId, int adminId)
        {
            try
            {
                // Check if admin
                if (!await IsUserAdminAsync(adminId))
                    return (false, "Only admins can unblock users");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return (false, "User not found");

                if (!user.isBlocked)
                    return (false, "User is not blocked");

                user.isBlocked = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {userId} unblocked by admin {adminId}");
                return (true, "User unblocked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unblocking user {userId}");
                return (false, "Error unblocking user");
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int userId, int adminId)
        {
            try
            {
                // Check if admin
                if (!await IsUserAdminAsync(adminId))
                    return (false, "Only admins can delete users");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return (false, "User not found");

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {userId} deleted by admin {adminId}");
                return (true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}");
                return (false, "Error deleting user");
            }
        }

        public async Task<(bool Success, string Message)> AddAdminRoleAsync(int userId, int adminId)
        {
            try
            {
                // Check if admin
                if (!await IsUserAdminAsync(adminId))
                    return (false, "Only admins can grant admin access");

                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return (false, "User not found");

                // Get admin role
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole == null)
                    return (false, "Admin role not found");

                // Check if already admin
                if (await IsUserAdminAsync(userId))
                    return (false, "User is already an admin");

                // Add admin role
                _context.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = adminRole.Id
                });

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Admin role added to user {userId} by admin {adminId}");
                return (true, "Admin role granted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding admin role to user {userId}");
                return (false, "Error granting admin role");
            }
        }

        public async Task<(bool Success, string Message)> RemoveAdminRoleAsync(int userId, int adminId)
        {
            try
            {
                // Check if admin
                if (!await IsUserAdminAsync(adminId))
                    return (false, "Only admins can remove admin access");

                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return (false, "User not found");

                // Get admin role
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole == null)
                    return (false, "Admin role not found");

                // Check if user is admin
                if (!await IsUserAdminAsync(userId))
                    return (false, "User is not an admin");

                // Remove admin role
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == adminRole.Id);

                if (userRole != null)
                {
                    _context.UserRoles.Remove(userRole);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"Admin role removed from user {userId} by admin {adminId}");
                return (true, "Admin role removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing admin role from user {userId}");
                return (false, "Error removing admin role");
            }
        }

        public async Task<bool> IsUserAdminAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin");
        }

        public async Task<List<UserSearchDto>> SearchUsersAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<UserSearchDto>();

            var lowerQuery = query.ToLower();

            return await _context.Users
                .Where(u => u.Username.ToLower().Contains(lowerQuery) || u.Email.ToLower().Contains(lowerQuery))
                .Take(10)
                .Select(u => new UserSearchDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .ToListAsync();
        }
    }
}
