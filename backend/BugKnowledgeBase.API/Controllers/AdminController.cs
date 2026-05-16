using BugKnowledgeBase.Application.DTOs.Categories;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")] // In a real app, map claims to roles
public class AdminController : ControllerBase
{
    private readonly IRepository<Category> _categoryRepository;

    public AdminController(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryDto dto, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId
        };
        
        await _categoryRepository.AddAsync(category, cancellationToken);
        dto.Id = category.Id;
        return Ok(dto);
    }
    
    // Additional admin endpoints (approve users, manage roles, etc.) would go here.
}
