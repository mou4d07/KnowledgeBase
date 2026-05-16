using System.ComponentModel.DataAnnotations;

namespace BugKnowledgeBase.Application.DTOs.Categories;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public int? ParentCategoryId { get; set; }
}
