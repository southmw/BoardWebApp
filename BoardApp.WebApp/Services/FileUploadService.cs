using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using BoardApp.WebApp.Data;
using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IConfiguration _configuration;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public FileUploadService(
        IWebHostEnvironment environment,
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IConfiguration configuration)
    {
        _environment = environment;
        _contextFactory = contextFactory;
        _configuration = configuration;
    }

    public async Task<BoardAttachment?> UploadFileAsync(IBrowserFile file, int boardId)
    {
        try
        {
            // 파일 크기 체크
            if (file.Size > MaxFileSize)
            {
                throw new Exception($"파일 크기는 {MaxFileSize / 1024 / 1024}MB를 초과할 수 없습니다.");
            }

            // 업로드 디렉토리 생성
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "boards", boardId.ToString());
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 파일명 생성 (중복 방지)
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.Name)}";
            var filePath = Path.Combine(uploadPath, fileName);

            // 파일 저장
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream(MaxFileSize).CopyToAsync(fileStream);

            // DB에 정보 저장
            var relativePath = Path.Combine("uploads", "boards", boardId.ToString(), fileName);
            var attachment = new BoardAttachment
            {
                BoardId = boardId,
                FileName = file.Name,
                FilePath = relativePath.Replace("\\", "/"),
                ContentType = file.ContentType,
                FileSize = file.Size,
                UploadedAt = DateTime.Now
            };

            using var context = await _contextFactory.CreateDbContextAsync();
            context.BoardAttachments.Add(attachment);
            await context.SaveChangesAsync();

            return attachment;
        }
        catch (Exception ex)
        {
            // 로깅 필요
            throw new Exception($"파일 업로드 실패: {ex.Message}");
        }
    }

    public async Task<List<BoardAttachment>> UploadFilesAsync(IReadOnlyList<IBrowserFile> files, int boardId)
    {
        var attachments = new List<BoardAttachment>();

        foreach (var file in files)
        {
            var attachment = await UploadFileAsync(file, boardId);
            if (attachment != null)
            {
                attachments.Add(attachment);
            }
        }

        return attachments;
    }

    public async Task<bool> DeleteFileAsync(int attachmentId)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var attachment = await context.BoardAttachments.FindAsync(attachmentId);

            if (attachment == null)
                return false;

            // 파일 삭제
            var fullPath = Path.Combine(_environment.WebRootPath, attachment.FilePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // DB에서 삭제
            context.BoardAttachments.Remove(attachment);
            await context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            var attachment = await context.BoardAttachments
                .FirstOrDefaultAsync(a => a.FilePath == filePath);

            if (attachment != null)
            {
                context.BoardAttachments.Remove(attachment);
                await context.SaveChangesAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetFileUrl(string filePath)
    {
        return $"/{filePath}";
    }

    public bool IsImageFile(string contentType)
    {
        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsVideoFile(string contentType)
    {
        return contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string?> UploadProfileImageAsync(IBrowserFile file, string userId)
    {
        try
        {
            // 이미지 파일만 허용
            if (!IsImageFile(file.ContentType))
            {
                throw new Exception("이미지 파일만 업로드할 수 있습니다.");
            }

            // 파일 크기 체크 (5MB)
            var maxSize = 5 * 1024 * 1024;
            if (file.Size > maxSize)
            {
                throw new Exception($"파일 크기는 {maxSize / 1024 / 1024}MB를 초과할 수 없습니다.");
            }

            // 업로드 디렉토리 생성
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 파일명 생성 (중복 방지)
            var extension = Path.GetExtension(file.Name);
            var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            // 파일 저장
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream(maxSize).CopyToAsync(fileStream);

            // 상대 경로 반환
            var relativePath = Path.Combine("uploads", "profiles", fileName);
            return relativePath.Replace("\\", "/");
        }
        catch (Exception ex)
        {
            throw new Exception($"프로필 이미지 업로드 실패: {ex.Message}");
        }
    }

    public async Task<bool> DeleteProfileImageAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return await Task.FromResult(true);
        }
        catch
        {
            return false;
        }
    }
}
