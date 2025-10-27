using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public interface IRoleService
{
    Task<List<ApplicationRole>> GetAllRolesAsync();
    Task<ApplicationRole?> GetRoleByIdAsync(string roleId);
    Task<ApplicationRole?> GetRoleByNameAsync(string roleName);
    Task<bool> CreateRoleAsync(ApplicationRole role);
    Task<bool> UpdateRoleAsync(ApplicationRole role);
    Task<bool> DeleteRoleAsync(string roleId);
    Task<int> GetUserCountInRoleAsync(string roleId);
    Task<bool> AddUserToRoleAsync(string userId, string roleName);
    Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
    Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName);
}
