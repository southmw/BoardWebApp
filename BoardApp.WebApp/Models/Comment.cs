using System.ComponentModel.DataAnnotations;

namespace BoardApp.WebApp.Models;

public class Comment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "댓글 내용은 필수입니다.")]
    [StringLength(1000, ErrorMessage = "댓글은 1000자를 초과할 수 없습니다.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "작성자는 필수입니다.")]
    [StringLength(50, ErrorMessage = "작성자명은 50자를 초과할 수 없습니다.")]
    public string Author { get; set; } = string.Empty;

    // User ID from Identity (nullable for backward compatibility)
    public string? AuthorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    // Foreign key
    public int BoardId { get; set; }

    // Navigation properties
    public Board Board { get; set; } = null!;
    public ApplicationUser? User { get; set; }
}
