using System.Net;
using Domain.Common;

namespace IntegrationTests;

[Collection("Integration")]
public class StatusControllerIntegrationTests : ApiTestBase
{
    public StatusControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetStatus_ReturnsApiStatus()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/api/Status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadResponseAsync<ApiStatus>(response);
        Assert.Equal("Online", payload.Status);
        Assert.Equal("integration-test-build", payload.BuildVersion);
    }
}
