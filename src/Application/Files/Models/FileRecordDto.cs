#nullable enable

using Domain.Entities;
using Domain.Enums;
using NodaTime;

namespace Application.Files.Models;

/// <summary>
/// Represents file metadata shown in the admin file table.
/// </summary>
public sealed class FileRecordDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileRecordDto"/> class.
    /// </summary>
    /// <param name="fileRecord">The source file metadata record.</param>
    public FileRecordDto(FileRecord fileRecord)
    {
        Id = fileRecord.Id;
        OriginalFileName = fileRecord.OriginalFileName;
        Key = fileRecord.ObjectKey;
        Url = fileRecord.Url;
        ContentType = fileRecord.ContentType;
        SizeBytes = fileRecord.SizeBytes;
        Category = fileRecord.Category;
        UploadedBy = fileRecord.CreatedBy;
        UploadedAt = fileRecord.CreatedDate;
    }

    /// <summary>
    /// Gets the metadata record identifier.
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// Gets the original file name supplied by the uploader.
    /// </summary>
    public string OriginalFileName { get; }

    /// <summary>
    /// Gets the storage object key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the public URL returned by storage.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Gets the uploaded file MIME type.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public long SizeBytes { get; }

    /// <summary>
    /// Gets the file category.
    /// </summary>
    public FileCategory Category { get; }

    /// <summary>
    /// Gets the user that uploaded the file.
    /// </summary>
    public string? UploadedBy { get; }

    /// <summary>
    /// Gets the upload timestamp.
    /// </summary>
    public Instant UploadedAt { get; }
}
