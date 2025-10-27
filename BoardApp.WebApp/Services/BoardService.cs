using Microsoft.EntityFrameworkCore;
using BoardApp.WebApp.Data;
using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public class BoardService : IBoardService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public BoardService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Board>> GetAllBoardsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Boards
            .Include(b => b.Category)
            .Include(b => b.Comments)
            .OrderByDescending(b => b.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Board?> GetBoardByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Boards
            .Include(b => b.Category)
            .Include(b => b.Attachments)
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Board> CreateBoardAsync(Board board)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        board.CreatedAt = DateTime.Now;
        context.Boards.Add(board);
        await context.SaveChangesAsync();
        return board;
    }

    public async Task<Board?> UpdateBoardAsync(int id, Board board)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existingBoard = await context.Boards.FindAsync(id);
        if (existingBoard == null)
            return null;

        existingBoard.Title = board.Title;
        existingBoard.Content = board.Content;
        existingBoard.Author = board.Author;
        existingBoard.CategoryId = board.CategoryId;
        existingBoard.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync();
        return existingBoard;
    }

    public async Task<bool> DeleteBoardAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var board = await context.Boards.FindAsync(id);
        if (board == null)
            return false;

        context.Boards.Remove(board);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task IncrementViewCountAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var board = await context.Boards.FindAsync(id);
        if (board != null)
        {
            board.ViewCount++;
            await context.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalCountAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Boards.CountAsync();
    }

    public async Task<List<Board>> GetPagedBoardsAsync(int pageNumber, int pageSize)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Boards
            .OrderByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<(List<Board> boards, int totalCount)> GetPagedBoardsWithCategoryAsync(int page, int pageSize, int? categoryId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        IQueryable<Board> query = context.Boards
            .Include(b => b.Category)
            .Include(b => b.Comments)
            .AsNoTracking();

        // 카테고리 필터 적용
        if (categoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
        }

        // 전체 게시글 수 계산
        int totalCount = await query.CountAsync();

        List<Board> boards;

        // 1페이지이고 전체 보기인 경우: 고정 게시글 + 일반 게시글
        if (page == 1 && !categoryId.HasValue)
        {
            // 고정 게시글 가져오기
            var pinnedBoards = await query
                .Where(b => b.Category.IsPinned)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            // 일반 게시글 가져오기
            var regularBoards = await query
                .Where(b => !b.Category.IsPinned)
                .OrderByDescending(b => b.CreatedAt)
                .Take(pageSize)
                .ToListAsync();

            boards = pinnedBoards.Concat(regularBoards).ToList();

            // totalCount는 일반 게시글 수만 계산 (고정 게시글 제외)
            totalCount = await query.Where(b => !b.Category.IsPinned).CountAsync();
        }
        else
        {
            // 다른 경우: 일반 페이징
            var skip = (page - 1) * pageSize;

            // 1페이지가 아니거나 카테고리 필터가 있는 경우
            if (page > 1 && !categoryId.HasValue)
            {
                // 일반 게시글만 페이징 (고정 게시글 제외)
                query = query.Where(b => !b.Category.IsPinned);
                totalCount = await query.CountAsync();
            }

            boards = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        return (boards, totalCount);
    }
}
