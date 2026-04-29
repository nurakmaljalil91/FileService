namespace Application.Files.Models;

/// <summary>
/// Represents the result of a successful file upload.
/// </summary>
/// <param name="Url">Public URL of the uploaded file.</param>
/// <param name="Key">Storage object key.</param>
/// <param name="ContentType">MIME type.</param>
/// <param name="SizeBytes">File size in bytes.</param>
public record UploadedFileDto(string Url, string Key, string ContentType, long SizeBytes);
