using BugKnowledgeBase.Application.DTOs.Comments;

namespace BugKnowledgeBase.Application.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetCommentsByBugIdAsync(int bugId, CancellationToken cancellationToken = default);
    Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, string createdBySessionName, CancellationToken cancellationToken = default);
    Task DeleteCommentAsync(int id, string deletedBySessionName, CancellationToken cancellationToken = default);
}
