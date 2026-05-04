using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Represents a test implementation of <see cref="IApplicationDbContext"/> using Entity Framework Core for unit testing purposes.
/// </summary>
public sealed class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the <see cref="DbSet{FileRecord}"/> representing uploaded file metadata.
    /// </summary>
    public DbSet<FileRecord> FileRecords => Set<FileRecord>();

    /// <summary>
    /// Configures the entity mappings for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileRecord>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Category).HasConversion<string>();
        });
    }
}
