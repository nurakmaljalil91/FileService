namespace Domain.Common;

/// <summary>
/// Represents the base entity with a unique identifier.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier (e.g. <see cref="long"/>, <see cref="System.Guid"/>).</typeparam>
public abstract class BaseEntity<TId>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public TId Id { get; set; } = default!;
}
