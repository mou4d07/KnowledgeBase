using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Comments;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;

namespace BugKnowledgeBase.Application.Services;

public class CommentService : ICommentService
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IMapper _mapper;

    public CommentService(IRepository<Comment> commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByBugIdAsync(int bugId, CancellationToken cancellationToken = default)
    {
        var comments = await _commentRepository.GetAsync(c => c.BugId == bugId, cancellationToken);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, string createdBySessionName, CancellationToken cancellationToken = default)
    {
        var comment = _mapper.Map<Comment>(dto);
        comment.CreatedBy = createdBySessionName;

        var created = await _commentRepository.AddAsync(comment, cancellationToken);
        return _mapper.Map<CommentDto>(created);
    }

    public async Task DeleteCommentAsync(int id, string deletedBySessionName, CancellationToken cancellationToken = default)
    {
        var comment = await _commentRepository.GetByIdAsync(id, cancellationToken);
        if (comment == null) return;

        await _commentRepository.DeleteSoftAsync(comment, deletedBySessionName, cancellationToken);
    }
}
