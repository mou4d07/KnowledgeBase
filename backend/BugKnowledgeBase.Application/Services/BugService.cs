using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Bugs;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;
using System.Linq;
using BugKnowledgeBase.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BugKnowledgeBase.Application.Services;

public class BugService : IBugService
{
    private readonly IRepository<Bug> _bugRepository;
    private readonly INotificationService _notificationService;
    private readonly IRepository<AuthorizedUser> _userRepository;
    private readonly IMapper _mapper;

    public BugService(
        IRepository<Bug> bugRepository, 
        INotificationService notificationService,
        IRepository<AuthorizedUser> userRepository,
        IMapper mapper)
    {
        _bugRepository = bugRepository;
        _notificationService = notificationService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<BugDto> GetBugByIdAsync(int id, string? currentUserSessionName = null, CancellationToken cancellationToken = default)
    {
        var bug = await _bugRepository.Query()
            .Include(b => b.Category)
            .Include(b => b.Attachments)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            
        if (bug == null) return null!;
        
        var dto = _mapper.Map<BugDto>(bug);
        await ApplyAccessControlAsync(dto, bug, currentUserSessionName, cancellationToken);
        return dto;
    }

    public async Task<IEnumerable<BugDto>> GetAllBugsAsync(string? currentUserSessionName = null, CancellationToken cancellationToken = default)
    {
        var bugs = await _bugRepository.Query()
            .Include(b => b.Category)
            .Include(b => b.Attachments)
            .ToListAsync(cancellationToken);
            
        var dtos = _mapper.Map<IEnumerable<BugDto>>(bugs).ToList();
        foreach (var dto in dtos)
        {
            var bug = bugs.First(b => b.Id == dto.Id);
            await ApplyAccessControlAsync(dto, bug, currentUserSessionName, cancellationToken);
        }
        return dtos;
    }

    private async Task ApplyAccessControlAsync(BugDto dto, Bug entity, string? currentUserSessionName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(entity.TargetStructure)) return;

        if (string.IsNullOrEmpty(currentUserSessionName))
        {
            dto.ClassifiedInformation = null;
            return;
        }

        var user = (await _userRepository.GetAsync(u => u.WindowsSessionName == currentUserSessionName, cancellationToken)).FirstOrDefault();
        if (user == null || (user.Role != "Admin" && user.Structure != entity.TargetStructure))
        {
            dto.ClassifiedInformation = null;
        }
    }

    public async Task<BugDto> CreateBugAsync(CreateBugDto dto, string createdBySessionName, CancellationToken cancellationToken = default)
    {
        var bug = _mapper.Map<Bug>(dto);
        bug.CreatedBy = createdBySessionName;
        bug.AuthorSessionName = createdBySessionName;
        
        var createdBug = await _bugRepository.AddAsync(bug, cancellationToken);

        // Notify Admins and Contributors
        var usersToNotify = await _userRepository.GetAsync(
            u => u.IsActive && (u.Role == "Admin" || u.Role == "Contributor") && u.WindowsSessionName != createdBySessionName, 
            cancellationToken);

        foreach (var user in usersToNotify)
        {
            await _notificationService.CreateNotificationAsync(
                user.WindowsSessionName,
                "New Bug Reported",
                $"A new bug '{createdBug.Title}' has been reported by {createdBySessionName}.",
                $"/bugs/{createdBug.Id}",
                cancellationToken);
        }

        return _mapper.Map<BugDto>(createdBug);
    }

    public async Task UpdateBugAsync(UpdateBugDto dto, string updatedBySessionName, CancellationToken cancellationToken = default)
    {
        var bug = await _bugRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (bug == null) return;
        
        _mapper.Map(dto, bug);
        bug.UpdatedBy = updatedBySessionName;
        
        await _bugRepository.UpdateAsync(bug, cancellationToken);

        if (!string.IsNullOrEmpty(bug.CreatedBy) && bug.CreatedBy != updatedBySessionName)
        {
            await _notificationService.CreateNotificationAsync(
                bug.CreatedBy,
                "Bug Updated",
                $"Your bug '{bug.Title}' has been updated by {updatedBySessionName}.",
                $"/bugs/{bug.Id}",
                cancellationToken);
        }
    }

    public async Task AssignBugAsync(int bugId, string? assigneeSessionName, string updatedBySessionName, CancellationToken cancellationToken = default)
    {
        var bug = await _bugRepository.GetByIdAsync(bugId, cancellationToken);
        if (bug == null) return;

        bug.AssignedToSessionName = assigneeSessionName;
        bug.UpdatedBy = updatedBySessionName;
        
        // If assigned and still New, move to InAnalysis
        if (!string.IsNullOrEmpty(assigneeSessionName) && bug.Status == BugStatus.New)
        {
            bug.Status = BugStatus.InAnalysis;
        }

        await _bugRepository.UpdateAsync(bug, cancellationToken);

        if (!string.IsNullOrEmpty(assigneeSessionName) && assigneeSessionName != updatedBySessionName)
        {
            await _notificationService.CreateNotificationAsync(
                assigneeSessionName,
                "Bug Assigned to You",
                $"Incident #{bug.Id} '{bug.Title}' has been assigned to you by {updatedBySessionName}.",
                $"/bug-detail?id={bug.Id}",
                cancellationToken);
        }
    }

    public async Task DeleteBugAsync(int id, string deletedBySessionName, CancellationToken cancellationToken = default)
    {
        var bug = await _bugRepository.GetByIdAsync(id, cancellationToken);
        if (bug == null) return;
        
        await _bugRepository.DeleteSoftAsync(bug, deletedBySessionName, cancellationToken);

        if (!string.IsNullOrEmpty(bug.CreatedBy) && bug.CreatedBy != deletedBySessionName)
        {
            await _notificationService.CreateNotificationAsync(
                bug.CreatedBy,
                "Bug Deleted",
                $"Your bug '{bug.Title}' has been deleted by {deletedBySessionName}.",
                null,
                cancellationToken);
        }
    }
}
