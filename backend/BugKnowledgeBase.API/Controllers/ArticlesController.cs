using BugKnowledgeBase.Application.DTOs.Articles;
using BugKnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<KnowledgeArticleDto>>> GetAllArticles(CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var articles = await _articleService.GetAllArticlesAsync(sessionName, cancellationToken);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<KnowledgeArticleDto>> GetArticleById(int id, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var article = await _articleService.GetArticleByIdAsync(id, sessionName, cancellationToken);
        if (article == null) return NotFound();
        return Ok(article);
    }

    [HttpPost]
    public async Task<ActionResult<KnowledgeArticleDto>> CreateArticle([FromBody] CreateKnowledgeArticleDto dto, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var created = await _articleService.CreateArticleAsync(dto, sessionName, cancellationToken);
        return CreatedAtAction(nameof(GetArticleById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateKnowledgeArticleDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id) return BadRequest();
        
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _articleService.UpdateArticleAsync(dto, sessionName, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _articleService.DeleteArticleAsync(id, sessionName, cancellationToken);
        
        return NoContent();
    }
    [HttpPatch("{id}/like")]
    public async Task<IActionResult> LikeArticle(int id, CancellationToken cancellationToken)
    {
        await _articleService.LikeArticleAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/dislike")]
    public async Task<IActionResult> DislikeArticle(int id, CancellationToken cancellationToken)
    {
        await _articleService.DislikeArticleAsync(id, cancellationToken);
        return NoContent();
    }
}
