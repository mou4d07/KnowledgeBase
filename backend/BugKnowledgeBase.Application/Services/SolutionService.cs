using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Solutions;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Enums;
using BugKnowledgeBase.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugKnowledgeBase.Application.Services;

public class SolutionService : ISolutionService
{
    private readonly IRepository<Solution> _solutionRepository;
    private readonly IRepository<Bug> _bugRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public SolutionService(
        IRepository<Solution> solutionRepository, 
        IRepository<Bug> bugRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _solutionRepository = solutionRepository;
        _bugRepository = bugRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SolutionDto>> GetAllSolutionsAsync(CancellationToken cancellationToken = default)
    {
        var solutions = await _solutionRepository.Query()
            .Include(s => s.Bug)
                .ThenInclude(b => b.Category)
            .ToListAsync(cancellationToken);
            
        return _mapper.Map<IEnumerable<SolutionDto>>(solutions);
    }

    public async Task<IEnumerable<SolutionDto>> GetSolutionsByBugIdAsync(int bugId, CancellationToken cancellationToken = default)
    {
        var solutions = await _solutionRepository.Query()
            .Include(s => s.Attachments)
            .Where(s => s.BugId == bugId)
            .ToListAsync(cancellationToken);
            
        return _mapper.Map<IEnumerable<SolutionDto>>(solutions);
    }

    public async Task<SolutionDto> CreateSolutionAsync(CreateSolutionDto dto, string createdBySessionName, CancellationToken cancellationToken = default)
    {
        var solution = _mapper.Map<Solution>(dto);
        solution.CreatedBy = createdBySessionName;
        // Business logic: if there is an existing solution, maybe increment version?
        // Simple version for now:
        var existing = await _solutionRepository.GetAsync(s => s.BugId == dto.BugId, cancellationToken);
        solution.Version = existing.Count == 0 ? 1 : existing.Max(s => s.Version) + 1;

        var created = await _solutionRepository.AddAsync(solution, cancellationToken);

        // Update Bug status to Resolved
        var bug = await _bugRepository.GetByIdAsync(dto.BugId, cancellationToken);
        if (bug != null)
        {
            bug.Status = BugStatus.Resolved;
            bug.ResolvedAt = DateTime.UtcNow;
            bug.ResolvedBy = createdBySessionName;
            await _bugRepository.UpdateAsync(bug, cancellationToken);

            if (!string.IsNullOrEmpty(bug.CreatedBy) && bug.CreatedBy != createdBySessionName)
            {
                await _notificationService.CreateNotificationAsync(
                    bug.CreatedBy,
                    "Solution Proposed",
                    $"A solution has been proposed for your bug '{bug.Title}' by {createdBySessionName}.",
                    $"/bugs/{bug.Id}",
                    cancellationToken);
            }
        }

        return _mapper.Map<SolutionDto>(created);
    }

    public async Task UpdateSolutionAsync(UpdateSolutionDto dto, string updatedBySessionName, CancellationToken cancellationToken = default)
    {
        var solution = await _solutionRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (solution == null) return;

        _mapper.Map(dto, solution);
        solution.UpdatedBy = updatedBySessionName;

        await _solutionRepository.UpdateAsync(solution, cancellationToken);

        if (!string.IsNullOrEmpty(solution.CreatedBy) && solution.CreatedBy != updatedBySessionName)
        {
            await _notificationService.CreateNotificationAsync(
                solution.CreatedBy,
                "Solution Modified",
                $"Your solution has been modified by {updatedBySessionName}.",
                $"/bugs/{solution.BugId}",
                cancellationToken);
        }
    }

    public async Task UpdateSolutionStatusAsync(int id, SolutionStatus status, string updatedBySessionName, CancellationToken cancellationToken = default)
    {
        var solution = await _solutionRepository.GetByIdAsync(id, cancellationToken);
        if (solution == null) return;

        solution.Status = status;
        solution.UpdatedBy = updatedBySessionName;
        await _solutionRepository.UpdateAsync(solution, cancellationToken);

        if (!string.IsNullOrEmpty(solution.CreatedBy) && solution.CreatedBy != updatedBySessionName)
        {
            await _notificationService.CreateNotificationAsync(
                solution.CreatedBy,
                "Solution Status Updated",
                $"Your solution status has been updated to {status} by {updatedBySessionName}.",
                $"/bugs/{solution.BugId}",
                cancellationToken);
        }
    }

    public async Task DeleteSolutionAsync(int id, string deletedBySessionName, CancellationToken cancellationToken = default)
    {
        var solution = await _solutionRepository.GetByIdAsync(id, cancellationToken);
        if (solution == null) return;
        
        await _solutionRepository.DeleteSoftAsync(solution, deletedBySessionName, cancellationToken);

        var bug = await _bugRepository.GetByIdAsync(solution.BugId, cancellationToken);
        if (bug != null && !string.IsNullOrEmpty(bug.CreatedBy) && bug.CreatedBy != deletedBySessionName)
        {
            await _notificationService.CreateNotificationAsync(
                bug.CreatedBy,
                "Solution Deleted",
                $"A solution for your bug '{bug.Title}' has been deleted by {deletedBySessionName}.",
                $"/bugs/{bug.Id}",
                cancellationToken);
        }
    }
}
