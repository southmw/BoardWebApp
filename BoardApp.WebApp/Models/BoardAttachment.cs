using System.ComponentModel.DataAnnotations;

namespace BoardApp.WebApp.Models;

public class BoardAttachment
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    // Navigation property
    public Board Board { get; set; } = null!;
}
