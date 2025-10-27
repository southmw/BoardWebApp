using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BoardApp.WebApp.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(50)]
    public string? DisplayName { get; set; }  // 닉네임/표시 이름

    [StringLength(500)]
    public string? Bio { get; set; }  // 자기소개

    [StringLength(200)]
    public string? ProfileImageUrl { get; set; }  // 프로필 이미지 URL

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
