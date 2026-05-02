#nullable enable

namespace Domain.Entities;

/// <summary>
/// Stores metadata for a file uploaded to object storage.
/// </summary>
public class FileRecord : BaseAuditableEntity<long>
{
    /// <summary>
    /// Gets or sets the original file name supplied by the uploader.
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object key used by the backing storage provider.
    /// </summary>
    public string ObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public URL returned after upload.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the uploaded file MIME type.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the category used for admin reporting.
    /// </summary>
    public FileCategory Category { get; set; }
}
