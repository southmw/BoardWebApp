using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public interface IBoardService
{
    Task<List<Board>> GetAllBoardsAsync();
    Task<Board?> GetBoardByIdAsync(int id);
    Task<Board> CreateBoardAsync(Board board);
    Task<Board?> UpdateBoardAsync(int id, Board board);
    Task<bool> DeleteBoardAsync(int id);
    Task IncrementViewCountAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<List<Board>> GetPagedBoardsAsync(int pageNumber, int pageSize);
    Task<(List<Board> boards, int totalCount)> GetPagedBoardsWithCategoryAsync(int page, int pageSize, int? categoryId = null);
}
