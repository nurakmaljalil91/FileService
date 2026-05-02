namespace Application.Files.Models;

using Domain.Entities;
using Domain.Enums;

/// <summary>
/// Represents the result of a successful file upload.
/// </summary>
/// <param name="Id">Metadata record identifier.</param>
/// <param name="Url">Public URL of the uploaded file.</param>
/// <param name="Key">Storage object key.</param>
/// <param name="OriginalFileName">Original file name supplied by the uploader.</param>
/// <param name="ContentType">MIME type.</param>
/// <param name="SizeBytes">File size in bytes.</param>
/// <param name="Category">File category used for admin reporting.</param>
public record UploadedFileDto(
    long Id,
    string Url,
    string Key,
    string OriginalFileName,
    string ContentType,
    long SizeBytes,
    FileCategory Category)
{
    /// <summary>
    /// Creates an upload DTO from a persisted file metadata record.
    /// </summary>
    /// <param name="fileRecord">The uploaded file metadata record.</param>
    /// <returns>The DTO representation.</returns>
    public static UploadedFileDto FromEntity(FileRecord fileRecord)
        => new(
            fileRecord.Id,
            fileRecord.Url,
            fileRecord.ObjectKey,
            fileRecord.OriginalFileName,
            fileRecord.ContentType,
            fileRecord.SizeBytes,
            fileRecord.Category);
}
