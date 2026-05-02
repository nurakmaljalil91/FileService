using Application.Common.Interfaces;
using Application.Files.Models;
using Domain.Common;
using Domain.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Files.Queries;

/// <summary>
/// Retrieves uploaded file counts and total sizes grouped by category.
/// </summary>
public sealed class GetFileCategorySummaryQuery : IRequest<BaseResponse<IReadOnlyCollection<FileCategorySummaryDto>>>;

/// <summary>
/// Handles retrieval of uploaded file category summaries.
/// </summary>
public sealed class GetFileCategorySummaryQueryHandler
    : IRequestHandler<GetFileCategorySummaryQuery, BaseResponse<IReadOnlyCollection<FileCategorySummaryDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFileCategorySummaryQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetFileCategorySummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<IReadOnlyCollection<FileCategorySummaryDto>>> Handle(
        GetFileCategorySummaryQuery request,
        CancellationToken cancellationToken)
    {
        var grouped = await _context.FileRecords
            .AsNoTracking()
            .GroupBy(x => x.Category)
            .Select(group => new FileCategorySummaryDto(
                group.Key,
                group.Count(),
                group.Sum(x => x.SizeBytes)))
            .ToListAsync(cancellationToken);

        var summaries = Enum.GetValues<FileCategory>()
            .GroupJoin(
                grouped,
                category => category,
                summary => summary.Category,
                (category, matches) => matches.SingleOrDefault() ?? new FileCategorySummaryDto(category, 0, 0))
            .ToArray();

        return BaseResponse<IReadOnlyCollection<FileCategorySummaryDto>>.Ok(
            summaries,
            "Successfully retrieved uploaded file category summary.");
    }
}
