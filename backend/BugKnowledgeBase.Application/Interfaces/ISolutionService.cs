using BugKnowledgeBase.Application.DTOs.Solutions;

namespace BugKnowledgeBase.Application.Interfaces;

public interface ISolutionService
{
    Task<IEnumerable<SolutionDto>> GetAllSolutionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SolutionDto>> GetSolutionsByBugIdAsync(int bugId, CancellationToken cancellationToken = default);
    Task<SolutionDto> CreateSolutionAsync(CreateSolutionDto dto, string createdBySessionName, CancellationToken cancellationToken = default);
    Task UpdateSolutionAsync(UpdateSolutionDto dto, string updatedBySessionName, CancellationToken cancellationToken = default);
    Task UpdateSolutionStatusAsync(int id, BugKnowledgeBase.Domain.Enums.SolutionStatus status, string updatedBySessionName, CancellationToken cancellationToken = default);
    Task DeleteSolutionAsync(int id, string deletedBySessionName, CancellationToken cancellationToken = default);
}
