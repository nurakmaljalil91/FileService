#nullable enable

using System.Reflection;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using WebAPI.Controllers;

namespace WebAPI.UnitTests.Controllers;

/// <summary>
/// Verifies authorization metadata exposed by <see cref="FilesController"/>.
/// </summary>
public sealed class FilesControllerAuthorizationTests
{
    /// <summary>
    /// Ensures platform claim roles can use the proxied attachment download flow.
    /// </summary>
    [Fact]
    public void Download_AllowsConfiguredPlatformDownloadRoles()
    {
        var method = typeof(FilesController).GetMethod(nameof(FilesController.Download));

        Assert.NotNull(method);
        var authorize = method!.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorize);
        Assert.Equal(FileAccessRoleConstants.Downloaders, authorize!.Roles);
    }
}
