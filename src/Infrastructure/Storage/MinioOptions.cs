namespace Infrastructure.Storage;

/// <summary>
/// Configuration options for the MinIO object storage backend.
/// Bound from the <c>Minio</c> section of appsettings.
/// </summary>
public class MinioOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Minio";

    /// <summary>MinIO API endpoint, e.g. <c>http://minio:9000</c>.</summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>Access key (root user).</summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>Secret key (root password).</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Target bucket name.</summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// Base URL used to build public object URLs, e.g.
    /// <c>http://localhost:9000/cerxos-media</c>.
    /// </summary>
    public string PublicBaseUrl { get; set; } = string.Empty;
}
