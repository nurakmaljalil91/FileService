#nullable enable
using Application.Files.Commands;
using Application.Files.Models;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Handles file upload operations.
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
    /// Uploads a file to object storage and returns its public URL.
    /// Accepts JPEG, PNG, or WebP images up to 5 MB.
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
}
