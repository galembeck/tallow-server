namespace Domain.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
    Task<List<string>> UploadMultipleFilesAsync(List<(Stream stream, string fileName)> files, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    string GetFileUrl(string filePath);
}