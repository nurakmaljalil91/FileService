namespace Infrastructure.Storage;

/// <summary>
/// Configuration options for an S3-compatible object storage backend.
/// Bound from the <c>Storage</c> section of appsettings.
/// </summary>
public class StorageOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Storage";

    /// <summary>S3-compatible storage API endpoint, e.g. <c>http://garage:3900</c>.</summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>Region used to sign S3 requests.</summary>
    public string Region { get; set; } = "garage";

    /// <summary>S3 access key.</summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>S3 secret key.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Target bucket name.</summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// Base URL used to build public object URLs, e.g.
    /// <c>http://localhost:9002</c>.
    /// </summary>
    public string PublicBaseUrl { get; set; } = string.Empty;
}
