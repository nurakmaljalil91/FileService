using Domain.Enums;

namespace Application.Files.Models;

/// <summary>
/// Represents aggregate upload metrics for a single file category.
/// </summary>
/// <param name="Category">The file category.</param>
/// <param name="Count">The number of uploaded files in the category.</param>
/// <param name="TotalSizeBytes">The total number of bytes uploaded in the category.</param>
public sealed record FileCategorySummaryDto(FileCategory Category, int Count, long TotalSizeBytes);
