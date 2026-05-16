using BugKnowledgeBase.Domain.Entities;

namespace BugKnowledgeBase.Application.Interfaces;

public interface IChatService
{
    Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string sessionName, CancellationToken cancellationToken = default);
    Task<ChatConversation> GetOrStartConversationAsync(string initiatorSessionName, string targetSessionName, CancellationToken cancellationToken = default);
    Task<ChatMessage> SendMessageAsync(int conversationId, string senderSessionName, string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatMessage>> GetConversationMessagesAsync(int conversationId, CancellationToken cancellationToken = default);
}
