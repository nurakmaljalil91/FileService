#nullable enable
using NodaTime;

namespace Domain.Common;

/// <summary>
/// Marker interface for auditable entities, allowing non-generic access to audit properties.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>Gets or sets the date and time when the entity was created.</summary>
    Instant CreatedDate { get; set; }

    /// <summary>Gets or sets the identifier of the user who created the entity.</summary>
    string? CreatedBy { get; set; }

    /// <summary>Gets or sets the date and time when the entity was last updated.</summary>
    Instant UpdatedDate { get; set; }

    /// <summary>Gets or sets the identifier of the user who last updated the entity.</summary>
    string? UpdatedBy { get; set; }
}
