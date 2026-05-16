using BugKnowledgeBase.Application.DTOs.Comments;
using BugKnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("bugs/{bugId}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForBug(int bugId, CancellationToken cancellationToken)
    {
        var comments = await _commentService.GetCommentsByBugIdAsync(bugId, cancellationToken);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment(int bugId, [FromBody] CreateCommentDto dto, CancellationToken cancellationToken)
    {
        if (bugId != dto.BugId) return BadRequest("Bug ID mismatch");

        var sessionName = User.Identity?.Name ?? "Unknown";
        var created = await _commentService.CreateCommentAsync(dto, sessionName, cancellationToken);
        return Ok(created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int bugId, int id, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _commentService.DeleteCommentAsync(id, sessionName, cancellationToken);
        return NoContent();
    }
}
