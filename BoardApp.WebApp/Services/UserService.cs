using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BoardApp.WebApp.Data;
using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        UserManager<ApplicationUser> userManager)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName)
    {
        return (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
    }

    public async Task<bool> UpdateUserAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<int> GetTotalUserCountAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.CountAsync();
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>();

        return (await _userManager.GetRolesAsync(user)).ToList();
    }
}
