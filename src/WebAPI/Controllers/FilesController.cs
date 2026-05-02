#nullable enable
using Application.Files.Commands;
using Application.Files.Models;
using Application.Files.Queries;
using Application.Common.Models;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Handles file upload and admin file management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="FilesController"/>.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Uploads a file to object storage and stores metadata for admin tracking.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result containing the public URL and metadata.</returns>
    [HttpPost("upload")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BaseResponse<UploadedFileDto>>> Upload(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var command = new UploadFileCommand
        {
            FileStream = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves uploaded file metadata for the admin file table.
    /// </summary>
    /// <param name="query">The query parameters for pagination, sorting, filtering, and category selection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of uploaded file metadata.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<FileRecordDto>>>> GetFiles(
        [FromQuery] GetFileRecordsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves upload counts and total sizes grouped by category for admins.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Summary metrics for image, video, audio, and document uploads.</returns>
    [HttpGet("summary")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<FileCategorySummaryDto>>>> GetSummary(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFileCategorySummaryQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Downloads an uploaded file by metadata identifier. Admin only.
    /// </summary>
    /// <param name="id">Uploaded file metadata identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The uploaded file stream.</returns>
    [HttpGet("{id:long}/download")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Download(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadFileQuery { Id = id }, cancellationToken);
        var file = result.Data!;

        return File(file.Content, file.ContentType, file.FileName);
    }
}
