using System.Net;

namespace IntegrationTests;

[Collection("Integration")]
public class FilesDownloadAuthorizationIntegrationTests : ApiTestBase
{
    private const string UnknownFileUrl = "/api/Files/999999/download";
    private const string NonDownloaderRole = "HREmployee";

    public FilesDownloadAuthorizationIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Download_WithServiceApiKey_PassesAuthorization()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Service-Api-Key", ApiFactory.ServiceApiKey);

        var response = await client.GetAsync(UnknownFileUrl);

        // The file does not exist, so getting past authorization surfaces as 404.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Download_WithServiceApiKeyAndNonDownloaderJwt_PassesAuthorization()
    {
        using var client = await CreateAuthenticatedClientAsync(NonDownloaderRole);
        client.DefaultRequestHeaders.Add("X-Service-Api-Key", ApiFactory.ServiceApiKey);

        var response = await client.GetAsync(UnknownFileUrl);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Download_WithWrongServiceApiKey_IsUnauthorized()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Service-Api-Key", "wrong-key");

        var response = await client.GetAsync(UnknownFileUrl);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Download_WithNonDownloaderJwtOnly_IsForbidden()
    {
        using var client = await CreateAuthenticatedClientAsync(NonDownloaderRole);

        var response = await client.GetAsync(UnknownFileUrl);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
