using BugKnowledgeBase.Application.DTOs.Bugs;
using BugKnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize] // Requires Windows Authentication (via our custom middleware)
public class BugsController : ControllerBase
{
    private readonly IBugService _bugService;

    public BugsController(IBugService bugService)
    {
        _bugService = bugService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BugDto>>> GetAllBugs(CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var bugs = await _bugService.GetAllBugsAsync(sessionName, cancellationToken);
        return Ok(bugs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BugDto>> GetBugById(int id, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var bug = await _bugService.GetBugByIdAsync(id, sessionName, cancellationToken);
        if (bug == null) return NotFound();
        return Ok(bug);
    }

    [HttpPost]
    public async Task<ActionResult<BugDto>> CreateBug([FromBody] CreateBugDto dto, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var createdBug = await _bugService.CreateBugAsync(dto, sessionName, cancellationToken);
        return CreatedAtAction(nameof(GetBugById), new { id = createdBug.Id }, createdBug);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBug(int id, [FromBody] UpdateBugDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id) return BadRequest();
        
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _bugService.UpdateBugAsync(dto, sessionName, cancellationToken);
        
        return NoContent();
    }

    [HttpPatch("{id}/assign")]
    public async Task<IActionResult> AssignBug(int id, [FromBody] string? assigneeSessionName, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _bugService.AssignBugAsync(id, assigneeSessionName, sessionName, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBug(int id, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _bugService.DeleteBugAsync(id, sessionName, cancellationToken);
        
        return NoContent();
    }
}
