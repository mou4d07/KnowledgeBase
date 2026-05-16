using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Chat;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IMapper _mapper;

    public ChatController(IChatService chatService, IMapper mapper)
    {
        _chatService = chatService;
        _mapper = mapper;
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<IEnumerable<ChatConversationDto>>> GetMyConversations(CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var conversations = await _chatService.GetUserConversationsAsync(sessionName, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<ChatConversationDto>>(conversations));
    }

    [HttpPost("start")]
    public async Task<ActionResult<ChatConversationDto>> StartConversation([FromBody] StartConversationDto dto, CancellationToken cancellationToken)
    {
        var sessionName = User.Identity?.Name ?? "Unknown";
        var conversation = await _chatService.GetOrStartConversationAsync(sessionName, dto.TargetSessionName, cancellationToken);
        return Ok(_mapper.Map<ChatConversationDto>(conversation));
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetMessages(int conversationId, CancellationToken cancellationToken)
    {
        // simplistic security check: does the conversation exist
        var messages = await _chatService.GetConversationMessagesAsync(conversationId, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<ChatMessageDto>>(messages));
    }

    [HttpGet("online-users")]
    public ActionResult<IEnumerable<string>> GetOnlineUsers()
    {
        return Ok(Hubs.ChatHub.GetOnlineUsers());
    }
}
