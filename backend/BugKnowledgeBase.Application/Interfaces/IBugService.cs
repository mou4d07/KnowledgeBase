using BugKnowledgeBase.Application.DTOs.Bugs;

namespace BugKnowledgeBase.Application.Interfaces;

public interface IBugService
{
    Task<BugDto> GetBugByIdAsync(int id, string? currentUserSessionName = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<BugDto>> GetAllBugsAsync(string? currentUserSessionName = null, CancellationToken cancellationToken = default);
    Task<BugDto> CreateBugAsync(CreateBugDto dto, string createdBySessionName, CancellationToken cancellationToken = default);
    Task UpdateBugAsync(UpdateBugDto dto, string updatedBySessionName, CancellationToken cancellationToken = default);
    Task AssignBugAsync(int bugId, string? assigneeSessionName, string updatedBySessionName, CancellationToken cancellationToken = default);
    Task DeleteBugAsync(int id, string deletedBySessionName, CancellationToken cancellationToken = default);
}
