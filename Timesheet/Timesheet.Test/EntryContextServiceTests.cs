using FakeItEasy;
using Timesheet.Core;
using Timesheet.Core.Db;
using Timesheet.Core.Models;
using Timesheet.Core.Resolvers;
using Timesheet.Core.Services;
using Timesheet.Test.Fixtures;

namespace Timesheet.Test;

public class EntryContextServiceTests : IClassFixture<DbFixture>
{
    private readonly DbFixture _fixture;

    public EntryContextServiceTests(DbFixture fixture)
    {
        _fixture = fixture;
        // Setup default user
        var userResolver = fixture.Get<IUserResolver>();
        A.CallTo(() => userResolver.Resolve()).Returns(Task.FromResult(new UserModel { Id = 1 }));
    }

    [Fact]
    public async Task GetEntryContextAsync_WithNullDescription_ReturnsFailure()
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();

        // Act
        var result = await service.GetEntryContextAsync(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultValues.TaskCodeNotFound, result.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task GetEntryContextAsync_WithEmptyOrWhitespaceDescription_ReturnsFailure(string description)
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();

        // Act
        var result = await service.GetEntryContextAsync(description);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultValues.TaskCodeNotFound, result.Code);
    }

    [Theory]
    [InlineData("[PROJ-1234] Working on feature", "PROJ-1234")]
    [InlineData("[PROJ-1234] Working on feature with spaces", "PROJ-1234")]
    [InlineData("[PROJ-1234_ABC] Working on feature", "PROJ-1234_ABC")]
    public async Task GetEntryContextAsync_WithSquareBracketsFormat_ExtractsTaskCode(string description, string expectedTaskCode)
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();

        // Act
        var result = await service.GetEntryContextAsync(description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTaskCode, result.Value.TaskCode);
    }

    [Theory]
    [InlineData("PROJ-1234: Working on feature", "PROJ-1234")]
    [InlineData("PROJ-1234: Working on feature with spaces", "PROJ-1234")]
    public async Task GetEntryContextAsync_WithColonFormat_ExtractsTaskCode(string description, string expectedTaskCode)
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();

        // Act
        var result = await service.GetEntryContextAsync(description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTaskCode, result.Value.TaskCode);
    }

    [Fact]
    public async Task GetEntryContextAsync_WithNoTaskCode_ReturnsFailure()
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();

        // Act
        var result = await service.GetEntryContextAsync("Just a description without task code");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultValues.TaskCodeNotFound, result.Code);
    }

    [Fact]
    public async Task GetEntryContextAsync_WithExistingTask_ReturnsExistingTask()
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();
        var taskCode = "PROJ-1234";
        var existingTask = new Core.Db.Entity.Task2
        {
            Code = taskCode,
            Description = "Existing task",
            UserId = 1
        };
        _fixture.Get<ApplicationDbContext>().Tasks.Add(existingTask);
        await _fixture.Get<ApplicationDbContext>().SaveChangesAsync();

        // Act
        var result = await service.GetEntryContextAsync($"[{taskCode}] Working on feature");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(existingTask.Id, result.Value.TaskId);
        Assert.Equal(taskCode, result.Value.TaskCode);
    }

    [Fact]
    public async Task GetEntryContextAsync_WithNewTask_CreatesNewTask()
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();
        var taskCode = "PROJ-1234";
        var description = "New task description";

        // Act
        var result = await service.GetEntryContextAsync($"[{taskCode}] {description}");

        // Assert
        Assert.True(result.IsSuccess);
        var createdTask = await _fixture.Get<ApplicationDbContext>().Tasks.FindAsync(result.Value.TaskId);
        Assert.NotNull(createdTask);
        Assert.Equal(taskCode, createdTask.Code);
        Assert.Equal(description, createdTask.Description);
        Assert.Equal(1, createdTask.UserId); // From userResolver
    }

    [Fact]
    public async Task GetEntryContextAsync_WithMultipleColons_ExtractsFirstPartAsTaskCode()
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();

        // Act
        var result = await service.GetEntryContextAsync("PROJ-1234: part1: part2");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PROJ-1234", result.Value.TaskCode);
    }

    [Fact]
    public async Task GetEntryContextAsync_WithMultipleSquareBrackets_UsesFirstBracketPair()
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();

        // Act
        var result = await service.GetEntryContextAsync("[PROJ-1234] [other] description");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PROJ-1234", result.Value.TaskCode);
    }

    [Fact]
    public async Task GetEntryContextAsync_WithUserResolutionFailure_HandlesError()
    {
        // Arrange
        var service = _fixture.Get<IEntryContextService>();
        A.CallTo(() => _fixture.Get<IUserResolver>().Resolve()).Throws<Exception>();

        // Act
        var result = await service.GetEntryContextAsync("[PROJ-1234] description");

        // Assert
        Assert.False(result.IsSuccess);
    }
}