using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugKnowledgeBase.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AttachmentsController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IRepository<BugAttachment> _bugAttachmentRepository;
    private readonly IRepository<SolutionAttachment> _solutionAttachmentRepository;
    private readonly IRepository<ArticleAttachment> _articleAttachmentRepository;

    public AttachmentsController(
        IFileService fileService,
        IRepository<BugAttachment> bugAttachmentRepository,
        IRepository<SolutionAttachment> solutionAttachmentRepository,
        IRepository<ArticleAttachment> articleAttachmentRepository)
    {
        _fileService = fileService;
        _bugAttachmentRepository = bugAttachmentRepository;
        _solutionAttachmentRepository = solutionAttachmentRepository;
        _articleAttachmentRepository = articleAttachmentRepository;
    }

    [HttpPost("bug/{bugId}")]
    public async Task<IActionResult> UploadBugAttachment(int bugId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty");

        using var stream = file.OpenReadStream();
        var relativePath = await _fileService.SaveFileAsync(stream, file.FileName, "bugs", cancellationToken);

        var attachment = new BugAttachment
        {
            BugId = bugId,
            FileName = file.FileName,
            FilePath = relativePath,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            CreatedBy = User.Identity?.Name ?? "Unknown"
        };

        await _bugAttachmentRepository.AddAsync(attachment, cancellationToken);
        return Ok(new { attachment.Id, attachment.FileName, attachment.FilePath, attachment.ContentType, attachment.FileSizeBytes });
    }

    [HttpPost("solution/{solutionId}")]
    public async Task<IActionResult> UploadSolutionAttachment(int solutionId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty");

        using var stream = file.OpenReadStream();
        var relativePath = await _fileService.SaveFileAsync(stream, file.FileName, "solutions", cancellationToken);

        var attachment = new SolutionAttachment
        {
            SolutionId = solutionId,
            FileName = file.FileName,
            FilePath = relativePath,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            CreatedBy = User.Identity?.Name ?? "Unknown"
        };

        await _solutionAttachmentRepository.AddAsync(attachment, cancellationToken);
        return Ok(new { attachment.Id, attachment.FileName, attachment.FilePath, attachment.ContentType, attachment.FileSizeBytes });
    }

    [HttpPost("article/{articleId}")]
    public async Task<IActionResult> UploadArticleAttachment(int articleId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty");

        using var stream = file.OpenReadStream();
        var relativePath = await _fileService.SaveFileAsync(stream, file.FileName, "articles", cancellationToken);

        var attachment = new ArticleAttachment
        {
            KnowledgeArticleId = articleId,
            FileName = file.FileName,
            FilePath = relativePath,
            ContentType = file.ContentType,
            FileSize = file.Length,
            CreatedBy = User.Identity?.Name ?? "Unknown"
        };

        await _articleAttachmentRepository.AddAsync(attachment, cancellationToken);
        return Ok(new { attachment.Id, attachment.FileName, attachment.FilePath, attachment.ContentType, attachment.FileSize });
    }

    [HttpGet("{id}/article")]
    public async Task<IActionResult> GetArticleAttachment(int id, CancellationToken cancellationToken)
    {
        var attachment = await _articleAttachmentRepository.GetByIdAsync(id, cancellationToken);
        if (attachment == null) return NotFound();

        var physicalPath = _fileService.GetPhysicalPath(attachment.FilePath);
        if (!System.IO.File.Exists(physicalPath)) return NotFound("File not found on disk");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(physicalPath, cancellationToken);
        return File(fileBytes, attachment.ContentType, attachment.FileName);
    }

    [HttpGet("{id}/bug")]
    public async Task<IActionResult> GetBugAttachment(int id, CancellationToken cancellationToken)
    {
        var attachment = await _bugAttachmentRepository.GetByIdAsync(id, cancellationToken);
        if (attachment == null) return NotFound();

        var physicalPath = _fileService.GetPhysicalPath(attachment.FilePath);
        if (!System.IO.File.Exists(physicalPath)) return NotFound("File not found on disk");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(physicalPath, cancellationToken);
        return File(fileBytes, attachment.ContentType, attachment.FileName);
    }

    [HttpGet("{id}/solution")]
    public async Task<IActionResult> GetSolutionAttachment(int id, CancellationToken cancellationToken)
    {
        var attachment = await _solutionAttachmentRepository.GetByIdAsync(id, cancellationToken);
        if (attachment == null) return NotFound();

        var physicalPath = _fileService.GetPhysicalPath(attachment.FilePath);
        if (!System.IO.File.Exists(physicalPath)) return NotFound("File not found on disk");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(physicalPath, cancellationToken);
        return File(fileBytes, attachment.ContentType, attachment.FileName);
    }
}
