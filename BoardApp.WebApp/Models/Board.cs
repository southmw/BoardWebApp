using System.ComponentModel.DataAnnotations;

namespace BoardApp.WebApp.Models;

public class Board
{
    public int Id { get; set; }

    [Required(ErrorMessage = "제목은 필수입니다.")]
    [StringLength(200, ErrorMessage = "제목은 200자를 초과할 수 없습니다.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "내용은 필수입니다.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "작성자는 필수입니다.")]
    [StringLength(50, ErrorMessage = "작성자명은 50자를 초과할 수 없습니다.")]
    public string Author { get; set; } = string.Empty;

    // User ID from Identity (nullable for backward compatibility)
    public string? AuthorId { get; set; }

    public int ViewCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    [Required(ErrorMessage = "카테고리는 필수입니다.")]
    public int CategoryId { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
    public ApplicationUser? User { get; set; }
    public ICollection<BoardAttachment> Attachments { get; set; } = new List<BoardAttachment>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
