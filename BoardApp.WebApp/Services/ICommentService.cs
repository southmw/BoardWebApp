using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public interface ICommentService
{
    Task<List<Comment>> GetCommentsByBoardIdAsync(int boardId);
    Task<Comment> CreateCommentAsync(Comment comment);
    Task UpdateCommentAsync(int id, Comment comment);
    Task DeleteCommentAsync(int id);
}
