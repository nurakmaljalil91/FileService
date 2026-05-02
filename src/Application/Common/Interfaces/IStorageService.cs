#nullable enable
namespace Application.Common.Interfaces;

/// <summary>
/// Defines a contract for uploading files to object storage.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Uploads a stream to object storage under the given key and returns the public URL.
    /// </summary>
    /// <param name="stream">The file content.</param>
    /// <param name="objectKey">The storage key, e.g. <c>profiles/abc.jpg</c>.</param>
    /// <param name="contentType">MIME type of the file.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The public URL of the uploaded object.</returns>
    Task<string> UploadAsync(Stream stream, string objectKey, string contentType, CancellationToken ct);

    /// <summary>
    /// Downloads an object from storage.
    /// </summary>
    /// <param name="objectKey">The storage key to download.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The downloaded content stream and metadata required for an HTTP file response.</returns>
    Task<StoredFileDownload> DownloadAsync(string objectKey, CancellationToken ct);
}

/// <summary>
/// Represents an object downloaded from storage.
/// </summary>
/// <param name="Content">The object content stream.</param>
/// <param name="ContentType">The object MIME type.</param>
/// <param name="Length">The object length in bytes.</param>
public sealed record StoredFileDownload(Stream Content, string ContentType, long Length);
