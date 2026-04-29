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
}
