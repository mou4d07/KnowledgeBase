using BugKnowledgeBase.Domain.Common;

namespace BugKnowledgeBase.Domain.Entities;

public class ChatConversation : BaseEntity
{
    public string? Title { get; set; } // Optional for group chats
    public bool IsGroup { get; set; }
    
    public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
