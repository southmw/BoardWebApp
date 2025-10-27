using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
    Task<bool> UpdateUserAsync(ApplicationUser user);
    Task<int> GetTotalUserCountAsync();
    Task<List<string>> GetUserRolesAsync(string userId);
}
