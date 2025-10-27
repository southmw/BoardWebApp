using Microsoft.AspNetCore.Components.Forms;
using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public interface IFileUploadService
{
    Task<BoardAttachment?> UploadFileAsync(IBrowserFile file, int boardId);
    Task<List<BoardAttachment>> UploadFilesAsync(IReadOnlyList<IBrowserFile> files, int boardId);
    Task<bool> DeleteFileAsync(int attachmentId);
    Task<bool> DeleteFileAsync(string filePath);
    Task<string?> UploadProfileImageAsync(IBrowserFile file, string userId);
    Task<bool> DeleteProfileImageAsync(string filePath);
    string GetFileUrl(string filePath);
    bool IsImageFile(string contentType);
    bool IsVideoFile(string contentType);
}
