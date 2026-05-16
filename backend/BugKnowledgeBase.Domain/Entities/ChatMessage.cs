using BugKnowledgeBase.Domain.Common;

namespace BugKnowledgeBase.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string SenderSessionName { get; set; } = string.Empty;

    public int ConversationId { get; set; }
    public ChatConversation Conversation { get; set; } = null!;
}
