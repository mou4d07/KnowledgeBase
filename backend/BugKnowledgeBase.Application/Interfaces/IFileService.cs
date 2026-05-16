namespace BugKnowledgeBase.Application.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string subFolder, CancellationToken cancellationToken = default);
    void DeleteFile(string filePath);
    string GetPhysicalPath(string relativePath);
}
