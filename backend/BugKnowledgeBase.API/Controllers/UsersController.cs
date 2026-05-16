using BugKnowledgeBase.Application.DTOs.Users;
using BugKnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;

    public UsersController(IUserService userService, IAuditService auditService)
    {
        _userService = userService;
        _auditService = auditService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<AuthorizedUserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var user = await _userService.GetUserBySessionNameAsync(sessionName, cancellationToken);
        
        if (user == null) return NotFound();

        // Log LOGIN only when profile is fetched (once per actual session start in frontend)
        await _auditService.LogActionAsync("LOGIN", "User", sessionName, null, null, sessionName, cancellationToken);

        return Ok(user);
    }

    [HttpGet("contacts")]
    public async Task<ActionResult<IEnumerable<AuthorizedUserDto>>> GetContacts(CancellationToken cancellationToken)
    {
        // Return all active users so users can pick who to chat with
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<AuthorizedUserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthorizedUserDto>> CreateUser([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var created = await _userService.CreateUserAsync(dto, cancellationToken);
        return Ok(created);
    }

    [HttpPut("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthorizedUserDto>> UpdateUser(int userId, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var updated = await _userService.UpdateUserAsync(userId, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int userId, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _userService.DeleteUserAsync(userId, sessionName, cancellationToken);
        return NoContent();
    }
}
