using Microsoft.EntityFrameworkCore;
using BoardApp.WebApp.Data;
using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public class CommentService : ICommentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CommentService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Comment>> GetCommentsByBoardIdAsync(int boardId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Comments
            .Where(c => c.BoardId == boardId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        comment.CreatedAt = DateTime.Now;
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task UpdateCommentAsync(int id, Comment comment)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existingComment = await context.Comments.FindAsync(id);
        if (existingComment != null)
        {
            existingComment.Content = comment.Content;
            existingComment.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteCommentAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var comment = await context.Comments.FindAsync(id);
        if (comment != null)
        {
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
    }
}
