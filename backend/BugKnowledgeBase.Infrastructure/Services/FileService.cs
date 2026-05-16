using BugKnowledgeBase.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BugKnowledgeBase.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _basePath;

    public FileService(IConfiguration configuration)
    {
        _basePath = configuration["FileStorage:BasePath"] ?? "wwwroot/uploads";
        if (!Path.IsPathRooted(_basePath))
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), _basePath);
        }

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string subFolder, CancellationToken cancellationToken = default)
    {
        var targetFolder = Path.Combine(_basePath, subFolder);
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var physicalPath = Path.Combine(targetFolder, uniqueFileName);
        
        using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream, cancellationToken);
        }

        // Return relative path for database storage
        return Path.Combine("uploads", subFolder, uniqueFileName).Replace("\\", "/");
    }

    public void DeleteFile(string filePath)
    {
        // filePath is relative, e.g., uploads/bugs/guid_name.jpg
        var relativePart = filePath.StartsWith("uploads/") ? filePath.Substring("uploads/".Length) : filePath;
        var physicalPath = Path.Combine(_basePath, relativePart);
        
        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
    }

    public string GetPhysicalPath(string relativePath)
    {
        var relativePart = relativePath.StartsWith("uploads/") ? relativePath.Substring("uploads/".Length) : relativePath;
        return Path.Combine(_basePath, relativePart);
    }
}
