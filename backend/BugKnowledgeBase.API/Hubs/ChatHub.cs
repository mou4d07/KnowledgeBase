using BugKnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BugKnowledgeBase.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, HashSet<string>> OnlineUsers = new();

    public override async Task OnConnectedAsync()
    {
        var sessionName = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(sessionName))
        {
            // Join a group specific to this user so we can easily message them directly
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionName);
            
            // Track online status
            bool isNewConnection = false;
            OnlineUsers.AddOrUpdate(sessionName,
                // Add new
                _ => {
                    isNewConnection = true;
                    return new HashSet<string> { Context.ConnectionId };
                },
                // Update existing
                (_, connections) => {
                    lock (connections)
                    {
                        if (connections.Count == 0) isNewConnection = true;
                        connections.Add(Context.ConnectionId);
                    }
                    return connections;
                });

            if (isNewConnection)
            {
                await Clients.All.SendAsync("UserConnected", sessionName);
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var sessionName = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(sessionName))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionName);
            
            if (OnlineUsers.TryGetValue(sessionName, out var connections))
            {
                bool isOffline = false;
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        isOffline = true;
                        OnlineUsers.TryRemove(sessionName, out _);
                    }
                }

                if (isOffline)
                {
                    await Clients.All.SendAsync("UserDisconnected", sessionName);
                }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    // Helper to get all currently online user session names
    public static IEnumerable<string> GetOnlineUsers()
    {
        return OnlineUsers.Keys;
    }

    public async Task SendMessage(int conversationId, string receiverSessionName, string message)
    {
        var senderSessionName = Context.User?.Identity?.Name ?? "Unknown";
        
        // Save to database
        var savedMessage = await _chatService.SendMessageAsync(conversationId, senderSessionName, message);

        // Send to receiver if online (user group)
        await Clients.Group(receiverSessionName).SendAsync("ReceiveMessage", conversationId, senderSessionName, message, savedMessage.CreatedAt);
        
        // Also send back to sender so other tabs sync (only if they aren't the same person)
        if (senderSessionName != receiverSessionName)
        {
            await Clients.Group(senderSessionName).SendAsync("ReceiveMessage", conversationId, senderSessionName, message, savedMessage.CreatedAt);
        }
    }
}
