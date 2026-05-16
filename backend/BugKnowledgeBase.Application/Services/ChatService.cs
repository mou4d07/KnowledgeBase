using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugKnowledgeBase.Application.Services;

public class ChatService : IChatService
{
    private readonly IRepository<ChatConversation> _conversationRepository;
    private readonly IRepository<ChatMessage> _messageRepository;
    private readonly IRepository<ChatParticipant> _participantRepository;

    public ChatService(
        IRepository<ChatConversation> conversationRepository,
        IRepository<ChatMessage> messageRepository,
        IRepository<ChatParticipant> participantRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _participantRepository = participantRepository;
    }

    public async Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string sessionName, CancellationToken cancellationToken = default)
    {
        var participations = await _participantRepository.GetAsync(p => p.WindowsSessionName == sessionName, cancellationToken);
        var conversationIds = participations.Select(p => p.ConversationId).ToList();
        
        // Very basic implementation. In production, want to include latest message, other participants.
        var conversations = await _conversationRepository.Query()
            .Include(c => c.Participants)
            .Where(c => conversationIds.Contains(c.Id))
            .ToListAsync(cancellationToken);
            
        return conversations;
    }

    public async Task<ChatConversation> GetOrStartConversationAsync(string initiatorSessionName, string targetSessionName, CancellationToken cancellationToken = default)
    {
        // simplistic 1-on-1 check with Include so we can actually see the participants
        var conv = await _conversationRepository.Query()
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => !c.IsGroup && 
                                      c.Participants.Any(p => p.WindowsSessionName == initiatorSessionName) &&
                                      c.Participants.Any(p => p.WindowsSessionName == targetSessionName), cancellationToken);

        if (conv != null) return conv;

        // Create new
        var newConv = new ChatConversation { IsGroup = false };
        newConv.Participants.Add(new ChatParticipant { WindowsSessionName = initiatorSessionName });
        newConv.Participants.Add(new ChatParticipant { WindowsSessionName = targetSessionName });

        return await _conversationRepository.AddAsync(newConv, cancellationToken);
    }

    public async Task<ChatMessage> SendMessageAsync(int conversationId, string senderSessionName, string content, CancellationToken cancellationToken = default)
    {
        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderSessionName = senderSessionName,
            Content = content
        };

        return await _messageRepository.AddAsync(message, cancellationToken);
    }

    public async Task<IEnumerable<ChatMessage>> GetConversationMessagesAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await _messageRepository.GetAsync(m => m.ConversationId == conversationId, cancellationToken);
    }
}
