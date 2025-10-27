using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<List<Category>> GetActiveCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category?> UpdateCategoryAsync(int id, Category category);
    Task<bool> DeleteCategoryAsync(int id);
    Task<int> GetBoardCountByCategoryAsync(int categoryId);
}
