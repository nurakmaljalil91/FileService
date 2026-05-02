using Infrastructure.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.UnitTests.Data;

/// <summary>
/// Unit tests for <see cref="ApplicationDbContextInitialiser"/>.
/// </summary>
public class ApplicationDbContextInitialiserTests
{
    /// <summary>
    /// Ensures the seed method adds default data when the database is empty.
    /// </summary>
    [Fact]
    public async Task TrySeedAsync_AddsDefaultDataWhenEmpty()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        var logger = NullLogger<ApplicationDbContextInitialiser>.Instance;
        var environment = new TestHostEnvironment { EnvironmentName = Environments.Development };
        var initialiser = new ApplicationDbContextInitialiser(logger, context, environment);

        await initialiser.TrySeedAsync();

        var todoList = await context.TodoLists.Include(list => list.Items).SingleAsync();

        Assert.Equal("Todo List", todoList.Title);
        Assert.Equal(4, todoList.Items.Count);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;

        public string ApplicationName { get; set; } = nameof(Infrastructure.UnitTests);

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
