using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BugKnowledgeBase.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var sessionName = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(sessionName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionName);
        }
        await base.OnConnectedAsync();
    }
}
