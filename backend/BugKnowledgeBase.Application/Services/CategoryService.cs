using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Categories;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;

namespace BugKnowledgeBase.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(IRepository<Category> categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var topLevel = categories.Where(c => c.ParentCategoryId == null).ToList();
        return _mapper.Map<IEnumerable<CategoryDto>>(topLevel);
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _categoryRepository.AddAsync(entity, cancellationToken);
        return _mapper.Map<CategoryDto>(created);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null) throw new KeyNotFoundException($"Category {id} not found.");

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.ParentCategoryId = dto.ParentCategoryId;
        entity.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<CategoryDto>(entity);
    }

    public async Task DeleteCategoryAsync(int id, string? deletedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null) throw new KeyNotFoundException($"Category {id} not found.");

        await _categoryRepository.DeleteSoftAsync(entity, deletedBy, cancellationToken);
    }
}

