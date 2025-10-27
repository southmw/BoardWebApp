using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BoardApp.WebApp.Data;
using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public class RoleService : IRoleService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _contextFactory = contextFactory;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<List<ApplicationRole>> GetAllRolesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Roles
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<ApplicationRole?> GetRoleByIdAsync(string roleId)
    {
        return await _roleManager.FindByIdAsync(roleId);
    }

    public async Task<ApplicationRole?> GetRoleByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<bool> CreateRoleAsync(ApplicationRole role)
    {
        role.CreatedAt = DateTime.Now;
        var result = await _roleManager.CreateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> UpdateRoleAsync(ApplicationRole role)
    {
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return false;

        // Check if any users have this role
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any())
        {
            return false; // Cannot delete role with users
        }

        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<int> GetUserCountInRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null || role.Name == null) return 0;

        var users = await _userManager.GetUsersInRoleAsync(role.Name);
        return users.Count;
    }

    public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName)
    {
        return (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
    }
}
