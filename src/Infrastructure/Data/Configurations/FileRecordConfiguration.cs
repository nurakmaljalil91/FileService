using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for uploaded file metadata.
/// </summary>
public class FileRecordConfiguration : IEntityTypeConfiguration<FileRecord>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ObjectKey)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(x => x.ObjectKey)
            .IsUnique();

        builder.Property(x => x.Url)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(127)
            .IsRequired();

        builder.Property(x => x.SizeBytes)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.CreatedDate);
    }
}
