#nullable enable

namespace Application.Files.Models;

/// <summary>
/// Represents a file stream ready to return from an HTTP download endpoint.
/// </summary>
/// <param name="Content">The downloaded file content.</param>
/// <param name="ContentType">The file MIME type.</param>
/// <param name="FileName">The original file name to present to the client.</param>
/// <param name="Length">The file length in bytes.</param>
public sealed record FileDownloadDto(Stream Content, string ContentType, string FileName, long Length);
