using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")] // Usually only admins view audit logs
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetRecentLogs([FromQuery] int count = 50, CancellationToken cancellationToken = default)
    {
        var logs = await _auditService.GetRecentAuditLogsAsync(count, cancellationToken);
        return Ok(logs);
    }
}
