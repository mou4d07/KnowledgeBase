using BugKnowledgeBase.Domain.Common;

namespace BugKnowledgeBase.Domain.Entities;

public class ChatParticipant : BaseEntity
{
    public int ConversationId { get; set; }
    public ChatConversation Conversation { get; set; } = null!;

    public string WindowsSessionName { get; set; } = string.Empty;
    public DateTime? LastReadAt { get; set; }
}
