#nullable enable

namespace Domain.Constants;

/// <summary>
/// Defines platform roles that may access stored file content.
/// </summary>
public static class FileAccessRoleConstants
{
    /// <summary>
    /// Role granted to trusted backend services authenticated by the shared service API key.
    /// </summary>
    public const string Service = "Service";

    /// <summary>
    /// Roles permitted to download files through FileService.
    /// </summary>
    public const string Downloaders = "Admin,Developer,SuperAdmin,ClaimAdministrator,ClaimStaff," + Service;
}
