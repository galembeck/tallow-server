using Domain.Constants;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Utils.Constants;
using Microsoft.AspNetCore.Hosting;

namespace Domain.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _baseUploadPath;
    private readonly string _baseUrl;

    public FileStorageService(IWebHostEnvironment env)
    {
        _baseUploadPath = Path.Combine(env.WebRootPath, "uploads");
        _baseUrl = Constant.Settings.BaseUrl;

        if (!Directory.Exists(_baseUploadPath))
            Directory.CreateDirectory(_baseUploadPath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        try
        {
            var folderPath = Path.Combine(_baseUploadPath, folder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            var relativePath = Path.Combine(folder, uniqueFileName).Replace("\\", "/");

            return relativePath;
        }
        catch (Exception ex)
        {
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
        }
    }

    public async Task<List<string>> UploadMultipleFilesAsync(List<(Stream stream, string fileName)> files, string folder)
    {
        var uploadedFiles = new List<string>();

        foreach (var file in files)
        {
            var filePath = await UploadFileAsync(file.stream, file.fileName, folder);
            uploadedFiles.Add(filePath);
        }

        return uploadedFiles;
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseUploadPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = Path.Combine(_baseUploadPath, filePath);

        return Task.FromResult(File.Exists(fullPath));
    }

    public string GetFileUrl(string filePath)
    {
        return $"{_baseUrl}/uploads/{filePath}";
    }
}
