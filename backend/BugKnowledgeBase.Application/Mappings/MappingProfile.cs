using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Bugs;
using BugKnowledgeBase.Application.DTOs.Categories;
using BugKnowledgeBase.Application.DTOs.Comments;
using BugKnowledgeBase.Application.DTOs.Solutions;
using BugKnowledgeBase.Application.DTOs.Users;
using BugKnowledgeBase.Application.DTOs.Common;
using BugKnowledgeBase.Application.DTOs.Articles;
using BugKnowledgeBase.Domain.Entities;

namespace BugKnowledgeBase.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Users
        CreateMap<AuthorizedUser, AuthorizedUserDto>();
        CreateMap<CreateUserDto, AuthorizedUser>()
            .ForMember(dest => dest.Structure, opt => opt.MapFrom(src => src.Structure));
        CreateMap<UpdateUserDto, AuthorizedUser>()
            .ForMember(dest => dest.Structure, opt => opt.MapFrom(src => src.Structure));

        // Categories
        CreateMap<Category, CategoryDto>();

        // Bugs
        CreateMap<Bug, BugDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<CreateBugDto, Bug>();
        CreateMap<UpdateBugDto, Bug>();

        // Solutions
        CreateMap<Solution, SolutionDto>()
            .ForMember(dest => dest.BugTitle, opt => opt.MapFrom(src => src.Bug.Title))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Bug.CategoryId))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Bug.Category.Name));
        CreateMap<CreateSolutionDto, Solution>();
        CreateMap<UpdateSolutionDto, Solution>();

        // Comments
        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentDto, Comment>();

        // Attachments
        CreateMap<BugAttachment, AttachmentDto>();
        CreateMap<SolutionAttachment, AttachmentDto>();
        CreateMap<ArticleAttachment, ArticleAttachmentDto>();

        // Articles
        CreateMap<KnowledgeArticle, KnowledgeArticleDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<CreateKnowledgeArticleDto, KnowledgeArticle>();
        CreateMap<UpdateKnowledgeArticleDto, KnowledgeArticle>();

        // Chat
        // Need to add using BugKnowledgeBase.Application.DTOs.Chat; at the top
        CreateMap<ChatConversation, BugKnowledgeBase.Application.DTOs.Chat.ChatConversationDto>();
        CreateMap<ChatParticipant, BugKnowledgeBase.Application.DTOs.Chat.ChatParticipantDto>();
        CreateMap<ChatMessage, BugKnowledgeBase.Application.DTOs.Chat.ChatMessageDto>();
    }
}
