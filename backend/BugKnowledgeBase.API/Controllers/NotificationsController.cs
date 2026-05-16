using BugKnowledgeBase.Application.DTOs.Notifications;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications(CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var notifications = await _notificationService.GetUserNotificationsAsync(sessionName, cancellationToken);
        
        var dtos = notifications.OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                LinkUrl = n.LinkUrl,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });

        return Ok(dtos);
    }

    [HttpGet("unreadCount")]
    public async Task<ActionResult<int>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var count = await _notificationService.GetUnreadCountAsync(sessionName, cancellationToken);
        return Ok(new { Count = count });
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _notificationService.MarkAsReadAsync(id, sessionName, cancellationToken);
        return NoContent();
    }

    [HttpPatch("readAll")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        await _notificationService.MarkAllAsReadAsync(sessionName, cancellationToken);
        return NoContent();
    }
}
