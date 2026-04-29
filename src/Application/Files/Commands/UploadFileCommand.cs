#nullable enable
using Application.Common.Interfaces;
using Application.Files.Models;
using Domain.Common;
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
    private static readonly string[] ImageContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private static readonly string[] VideoContentTypes = ["video/mp4", "video/webm", "video/quicktime"];
    private static readonly string[] AllowedContentTypes = [..ImageContentTypes, ..VideoContentTypes];

    private const long MaxImageSizeBytes = 5L * 1024 * 1024;       // 5 MB
    private const long MaxVideoSizeBytes = 500L * 1024 * 1024;     // 500 MB

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
            .WithMessage("Only JPEG, PNG, WebP, MP4, WebM, and QuickTime files are allowed.");

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
    }
}

/// <summary>
/// Handles <see cref="UploadFileCommand"/> by writing the file to object storage.
/// </summary>
public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, BaseResponse<UploadedFileDto>>
{
    private readonly IStorageService _storage;

    /// <summary>
    /// Initializes a new instance of <see cref="UploadFileCommandHandler"/>.
    /// </summary>
    /// <param name="storage">Storage service abstraction.</param>
    public UploadFileCommandHandler(IStorageService storage)
    {
        _storage = storage;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<UploadedFileDto>> Handle(
        UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.FileName);
        var folder = request.ContentType!.StartsWith("video/") ? "videos" : "profiles";
        var objectKey = $"{folder}/{Guid.NewGuid()}{extension}";

        var url = await _storage.UploadAsync(
            request.FileStream!,
            objectKey,
            request.ContentType!,
            cancellationToken);

        var dto = new UploadedFileDto(url, objectKey, request.ContentType!, request.SizeBytes);

        return BaseResponse<UploadedFileDto>.Ok(dto, "File uploaded successfully.");
    }
}
