#nullable enable

using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Files.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Files.Queries;

/// <summary>
/// Downloads an uploaded file by metadata identifier.
/// </summary>
public sealed class DownloadFileQuery : IRequest<BaseResponse<FileDownloadDto>>
{
    /// <summary>
    /// Gets or sets the uploaded file metadata identifier.
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// Handles admin file downloads.
/// </summary>
public sealed class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, BaseResponse<FileDownloadDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStorageService _storage;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadFileQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="storage">The storage service abstraction.</param>
    public DownloadFileQueryHandler(IApplicationDbContext context, IStorageService storage)
    {
        _context = context;
        _storage = storage;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<FileDownloadDto>> Handle(
        DownloadFileQuery request,
        CancellationToken cancellationToken)
    {
        var fileRecord = await _context.FileRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (fileRecord is null)
        {
            throw new NotFoundException(nameof(fileRecord), request.Id.ToString());
        }

        var storedFile = await _storage.DownloadAsync(fileRecord.ObjectKey, cancellationToken);
        var dto = new FileDownloadDto(
            storedFile.Content,
            string.IsNullOrWhiteSpace(storedFile.ContentType) ? fileRecord.ContentType : storedFile.ContentType,
            fileRecord.OriginalFileName,
            storedFile.Length);

        return BaseResponse<FileDownloadDto>.Ok(dto, "File downloaded successfully.");
    }
}
