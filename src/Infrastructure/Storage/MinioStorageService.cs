#nullable enable
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

/// <summary>
/// <see cref="IStorageService"/> implementation backed by MinIO via the AWS S3 SDK.
/// </summary>
public class MinioStorageService : IStorageService
{
    private readonly AmazonS3Client _client;
    private readonly MinioOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="MinioStorageService"/>.
    /// </summary>
    /// <param name="options">MinIO configuration.</param>
    public MinioStorageService(IOptions<MinioOptions> options)
    {
        _options = options.Value;

        var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
        var config = new AmazonS3Config
        {
            ServiceURL = _options.Endpoint,
            ForcePathStyle = true   // required for MinIO path-style addressing
        };

        _client = new AmazonS3Client(credentials, config);
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(
        Stream stream,
        string objectKey,
        string contentType,
        CancellationToken ct)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await _client.PutObjectAsync(request, ct);

        return $"{_options.PublicBaseUrl}/{objectKey}";
    }

    /// <inheritdoc />
    public async Task<StoredFileDownload> DownloadAsync(string objectKey, CancellationToken ct)
    {
        var response = await _client.GetObjectAsync(_options.Bucket, objectKey, ct);

        return new StoredFileDownload(
            response.ResponseStream,
            response.Headers.ContentType,
            response.Headers.ContentLength);
    }
}
