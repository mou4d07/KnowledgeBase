using BugKnowledgeBase.Application.DTOs.Users;

namespace BugKnowledgeBase.Application.Interfaces;

public interface IUserService
{
    Task<AuthorizedUserDto?> GetUserBySessionNameAsync(string sessionName, CancellationToken cancellationToken = default);
    Task<bool> IsUserAuthorizedAsync(string sessionName, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuthorizedUserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<AuthorizedUserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<AuthorizedUserDto> UpdateUserAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
