namespace BugKnowledgeBase.Application.DTOs.Chat;

public class ChatParticipantDto
{
    public int Id { get; set; }
    public string WindowsSessionName { get; set; } = string.Empty;
}

public class ChatMessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string SenderSessionName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ChatConversationDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsGroup { get; set; }
    public List<ChatParticipantDto> Participants { get; set; } = new();
}

public class StartConversationDto
{
    public string TargetSessionName { get; set; } = string.Empty;
}
