using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Users;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;
using System.Collections.Generic;

namespace BugKnowledgeBase.Application.Services;

public class UserService : IUserService
{
    private readonly IRepository<AuthorizedUser> _userRepository;
    private readonly IMapper _mapper;

    public UserService(IRepository<AuthorizedUser> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AuthorizedUserDto?> GetUserBySessionNameAsync(string sessionName, CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.GetAsync(u => u.WindowsSessionName == sessionName, cancellationToken)).FirstOrDefault();
        return user == null ? null : _mapper.Map<AuthorizedUserDto>(user);
    }

    public async Task<bool> IsUserAuthorizedAsync(string sessionName, CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.GetAsync(u => u.WindowsSessionName == sessionName && u.IsActive, cancellationToken)).FirstOrDefault();
        return user != null;
    }

    public async Task<IEnumerable<AuthorizedUserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<AuthorizedUserDto>>(users);
    }

    public async Task<AuthorizedUserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = _mapper.Map<AuthorizedUser>(dto);
        user.Structure = dto.Structure ?? string.Empty;
        var created = await _userRepository.AddAsync(user, cancellationToken);
        return _mapper.Map<AuthorizedUserDto>(created);
    }

    public async Task<AuthorizedUserDto> UpdateUserAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null) throw new KeyNotFoundException("User not found");

        _mapper.Map(dto, user);
        user.Structure = dto.Structure ?? string.Empty;
        await _userRepository.UpdateAsync(user, cancellationToken);
        return _mapper.Map<AuthorizedUserDto>(user);
    }

    public async Task DeleteUserAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null) throw new KeyNotFoundException("User not found");

        await _userRepository.DeleteSoftAsync(user, deletedBy, cancellationToken);
    }
}
