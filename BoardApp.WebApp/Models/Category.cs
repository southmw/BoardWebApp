using System.ComponentModel.DataAnnotations;

namespace BoardApp.WebApp.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "카테고리명은 필수입니다.")]
    [StringLength(50, ErrorMessage = "카테고리명은 50자를 초과할 수 없습니다.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "설명은 200자를 초과할 수 없습니다.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "색상 코드는 필수입니다.")]
    [StringLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "올바른 색상 코드를 입력하세요. (예: #667eea)")]
    public string Color { get; set; } = "#667eea";

    public int DisplayOrder { get; set; } = 0;

    public bool IsPinned { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // User ID from Identity (nullable)
    public string? CreatedById { get; set; }

    // Navigation properties
    public ApplicationUser? CreatedBy { get; set; }
    public ICollection<Board> Boards { get; set; } = new List<Board>();
}
