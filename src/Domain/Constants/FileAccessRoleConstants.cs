#nullable enable

namespace Domain.Constants;

/// <summary>
/// Defines platform roles that may access stored file content.
/// </summary>
public static class FileAccessRoleConstants
{
    /// <summary>
    /// Roles permitted to download files through FileService.
    /// </summary>
    public const string Downloaders = "Admin,Developer,SuperAdmin,ClaimAdministrator,ClaimStaff";
}
