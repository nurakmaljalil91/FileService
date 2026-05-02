#nullable enable

using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Files.Models;
using Domain.Common;
using Domain.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Files.Queries;

/// <summary>
/// Retrieves uploaded file metadata for the admin file table.
/// </summary>
public sealed class GetFileRecordsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<FileRecordDto>>>
{
    /// <summary>
    /// Gets or sets the optional file category filter.
    /// </summary>
    public FileCategory? Category { get; set; }
}

/// <summary>
/// Handles retrieval of uploaded file metadata.
/// </summary>
public sealed class GetFileRecordsQueryHandler : IRequestHandler<GetFileRecordsQuery, BaseResponse<PaginatedEnumerable<FileRecordDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFileRecordsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetFileRecordsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<PaginatedEnumerable<FileRecordDto>>> Handle(
        GetFileRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.FileRecords.AsNoTracking();

        if (request.Category.HasValue)
        {
            query = query.Where(x => x.Category == request.Category.Value);
        }

        query = query
            .ApplyFilters(request.Filter)
            .ApplySorting(
                string.IsNullOrWhiteSpace(request.SortBy) ? nameof(Domain.Entities.FileRecord.CreatedDate) : request.SortBy,
                string.IsNullOrWhiteSpace(request.SortBy) || request.Descending);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;
        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(fileRecord => new FileRecordDto(fileRecord))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedEnumerable<FileRecordDto>(result, totalCount, page, pageSize);

        return BaseResponse<PaginatedEnumerable<FileRecordDto>>.Ok(
            paginatedResult,
            $"Successfully retrieved {paginatedResult.Items?.Count() ?? 0} uploaded files.");
    }
}
