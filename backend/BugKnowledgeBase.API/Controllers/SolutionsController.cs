using BugKnowledgeBase.Application.DTOs.Solutions;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("bugs/{bugId}/solutions")]
[Authorize]
public class SolutionsController : ControllerBase
{
    private readonly ISolutionService _solutionService;

    public SolutionsController(ISolutionService solutionService)
    {
        _solutionService = solutionService;
    }

    [HttpGet("/api/solutions")]
    public async Task<ActionResult<IEnumerable<SolutionDto>>> GetAllSolutions(CancellationToken cancellationToken)
    {
        var solutions = await _solutionService.GetAllSolutionsAsync(cancellationToken);
        return Ok(solutions);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolutionDto>>> GetSolutionsForBug(int bugId, CancellationToken cancellationToken)
    {
        var solutions = await _solutionService.GetSolutionsByBugIdAsync(bugId, cancellationToken);
        return Ok(solutions);
    }

    [HttpPost]
    public async Task<ActionResult<SolutionDto>> CreateSolution(int bugId, [FromBody] CreateSolutionDto dto, CancellationToken cancellationToken)
    {
        if (bugId != dto.BugId) return BadRequest("Bug ID mismatch");

        var sessionName = User.Identity?.Name ?? "Unknown";
        var created = await _solutionService.CreateSolutionAsync(dto, sessionName, cancellationToken);
        return Ok(created); // Simplification for now instead of CreatedAtAction
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSolution(int bugId, int id, [FromBody] UpdateSolutionDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");

        var sessionName = User.Identity?.Name ?? "Unknown";
        await _solutionService.UpdateSolutionAsync(dto, sessionName, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateSolutionStatus(int bugId, int id, [FromBody] SolutionStatus status, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _solutionService.UpdateSolutionStatusAsync(id, status, sessionName, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSolution(int bugId, int id, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _solutionService.DeleteSolutionAsync(id, sessionName, cancellationToken);
        return NoContent();
    }
}
