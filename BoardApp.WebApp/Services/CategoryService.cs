using Microsoft.EntityFrameworkCore;
using BoardApp.WebApp.Data;
using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public class CategoryService : ICategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CategoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Categories
            .OrderBy(c => c.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Category>> GetActiveCategoriesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        category.CreatedAt = DateTime.Now;
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateCategoryAsync(int id, Category category)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existingCategory = await context.Categories.FindAsync(id);
        if (existingCategory == null)
        {
            return null;
        }

        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;
        existingCategory.Color = category.Color;
        existingCategory.DisplayOrder = category.DisplayOrder;
        existingCategory.IsPinned = category.IsPinned;
        existingCategory.IsActive = category.IsActive;

        await context.SaveChangesAsync();
        return existingCategory;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // 해당 카테고리의 게시글이 있는지 확인
        var boardCount = await context.Boards.CountAsync(b => b.CategoryId == id);
        if (boardCount > 0)
        {
            return false; // 게시글이 있으면 삭제 불가
        }

        var category = await context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetBoardCountByCategoryAsync(int categoryId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Boards.CountAsync(b => b.CategoryId == categoryId);
    }
}
