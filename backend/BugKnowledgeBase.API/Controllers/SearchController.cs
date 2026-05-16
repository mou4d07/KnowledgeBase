using BugKnowledgeBase.Application.DTOs.Bugs;
using BugKnowledgeBase.Application.DTOs.Search;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchResultDto>>> Search(
        [FromQuery] string? keyword,
        [FromQuery] int? categoryId,
        [FromQuery] Severity? severity,
        [FromQuery] EnvironmentType? environment,
        CancellationToken cancellationToken)
    {
        var results = await _searchService.SearchBugsAsync(keyword, categoryId, severity, environment, cancellationToken);
        return Ok(results);
    }
}
