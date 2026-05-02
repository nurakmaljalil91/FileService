#nullable enable
using Application.Common.Interfaces;
using Application.Files.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Mediator;

namespace Application.Files.Commands;

/// <summary>
/// Command to upload a file to object storage.
/// </summary>
public class UploadFileCommand : IRequest<BaseResponse<UploadedFileDto>>
{
    /// <summary>Raw file content stream.</summary>
    public Stream? FileStream { get; set; }

    /// <summary>Original file name (used to derive the extension).</summary>
    public string? FileName { get; set; }

    /// <summary>MIME type of the file.</summary>
    public string? ContentType { get; set; }

    /// <summary>File size in bytes.</summary>
    public long SizeBytes { get; set; }
}

/// <summary>
/// Validates <see cref="UploadFileCommand"/> before execution.
/// </summary>
public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    private static readonly string[] ImageContentTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];
    private static readonly string[] VideoContentTypes = ["video/mp4", "video/webm", "video/quicktime"];
    private static readonly string[] AudioContentTypes = ["audio/mpeg", "audio/mp4", "audio/ogg", "audio/wav", "audio/webm", "audio/aac"];
    private static readonly string[] DocumentContentTypes =
    [
        "application/pdf",
        "text/plain",
        "text/csv",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation"
    ];
    private static readonly string[] AllowedContentTypes = [..ImageContentTypes, ..VideoContentTypes, ..AudioContentTypes, ..DocumentContentTypes];

    private const long MaxImageSizeBytes = 5L * 1024 * 1024;       // 5 MB
    private const long MaxVideoSizeBytes = 500L * 1024 * 1024;     // 500 MB
    private const long MaxAudioSizeBytes = 100L * 1024 * 1024;     // 100 MB
    private const long MaxDocumentSizeBytes = 25L * 1024 * 1024;   // 25 MB

    /// <summary>
    /// Initializes validation rules.
    /// </summary>
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File is required.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.");

        RuleFor(x => x.ContentType)
            .Must(ct => ct != null && AllowedContentTypes.Contains(ct))
            .WithMessage("Only supported image, video, audio, and document files are allowed.");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0).WithMessage("File cannot be empty.");

        When(x => x.ContentType != null && ImageContentTypes.Contains(x.ContentType), () =>
        {
            RuleFor(x => x.SizeBytes)
                .LessThanOrEqualTo(MaxImageSizeBytes)
                .WithMessage("Images must be 5 MB or smaller.");
        });

        When(x => x.ContentType != null && VideoContentTypes.Contains(x.ContentType), () =>
        {
            RuleFor(x => x.SizeBytes)
                .LessThanOrEqualTo(MaxVideoSizeBytes)
                .WithMessage("Videos must be 500 MB or smaller.");
        });

        When(x => x.ContentType != null && AudioContentTypes.Contains(x.ContentType), () =>
        {
            RuleFor(x => x.SizeBytes)
                .LessThanOrEqualTo(MaxAudioSizeBytes)
                .WithMessage("Audio files must be 100 MB or smaller.");
        });

        When(x => x.ContentType != null && DocumentContentTypes.Contains(x.ContentType), () =>
        {
            RuleFor(x => x.SizeBytes)
                .LessThanOrEqualTo(MaxDocumentSizeBytes)
                .WithMessage("Documents must be 25 MB or smaller.");
        });
    }
}

/// <summary>
/// Handles <see cref="UploadFileCommand"/> by writing the file to object storage.
/// </summary>
public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, BaseResponse<UploadedFileDto>>
{
    private readonly IStorageService _storage;
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="UploadFileCommandHandler"/>.
    /// </summary>
    /// <param name="storage">Storage service abstraction.</param>
    /// <param name="context">Application database context.</param>
    public UploadFileCommandHandler(IStorageService storage, IApplicationDbContext context)
    {
        _storage = storage;
        _context = context;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<UploadedFileDto>> Handle(
        UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.FileName);
        var category = ResolveCategory(request.ContentType!);
        var folder = ResolveFolder(category);
        var objectKey = $"{folder}/{Guid.NewGuid()}{extension}";

        var url = await _storage.UploadAsync(
            request.FileStream!,
            objectKey,
            request.ContentType!,
            cancellationToken);

        var fileRecord = new FileRecord
        {
            OriginalFileName = request.FileName!,
            ObjectKey = objectKey,
            Url = url,
            ContentType = request.ContentType!,
            SizeBytes = request.SizeBytes,
            Category = category
        };

        _context.FileRecords.Add(fileRecord);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = UploadedFileDto.FromEntity(fileRecord);

        return BaseResponse<UploadedFileDto>.Ok(dto, "File uploaded successfully.");
    }

    private static FileCategory ResolveCategory(string contentType)
    {
        if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return FileCategory.Image;
        }

        if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
        {
            return FileCategory.Video;
        }

        if (contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
        {
            return FileCategory.Audio;
        }

        return FileCategory.Document;
    }

    private static string ResolveFolder(FileCategory category)
        => category switch
        {
            FileCategory.Image => "images",
            FileCategory.Video => "videos",
            FileCategory.Audio => "audios",
            FileCategory.Document => "documents",
            _ => "files"
        };
}
