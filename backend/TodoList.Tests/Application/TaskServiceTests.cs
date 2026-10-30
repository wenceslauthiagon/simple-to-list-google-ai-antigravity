using TodoList.Application.DTOs;
using TodoList.Application.Exceptions;
using TodoList.Application.Services;
using TodoList.Domain.Constants;
using TodoList.Domain.Enums;
using TodoList.Tests.Fakes;
using Xunit;

namespace TodoList.Tests.Application;

public class TaskServiceTests
{
    private readonly InMemoryTaskRepository _repository;
    private readonly TestTimeProvider _timeProvider;
    private readonly TaskService _service;
    private readonly DateTimeOffset _baseTime = new(2026, 10, 29, 10, 0, 0, TimeSpan.Zero);

    public TaskServiceTests()
    {
        _repository = new InMemoryTaskRepository();
        _timeProvider = new TestTimeProvider(_baseTime);
        _service = new TaskService(_repository, _timeProvider);
    }

    [Fact(DisplayName = "TC-APP-0001 - Create valid task persists entity and returns response DTO")]
    public async Task CreateTaskAsync_WithValidRequest_CreatesAndReturnsTask()
    {
        var request = TaskFaker.CreateRequest("Write clean code", TaskPriority.High);

        var result = await _service.CreateTaskAsync(request);

        Assert.NotNull(result);
        Assert.Equal("Write clean code", result.Title);
        Assert.Equal(TaskPriority.High, result.Priority);
        Assert.Equal(TaskStatus.Pending, result.Status);
        Assert.Equal(_baseTime, result.CreatedAt);
        Assert.Null(result.UpdatedAt);

        var saved = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("Write clean code", saved.Title);
    }

    [Theory(DisplayName = "TC-APP-0002 - Create task with null or whitespace title throws validation exception")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateTaskAsync_WithNullOrWhitespaceTitle_ThrowsValidationException(string? invalidTitle)
    {
        var request = new CreateTaskRequest(invalidTitle!, TaskPriority.Medium);

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateTaskAsync(request));
    }

    [Fact(DisplayName = "TC-APP-0003 - Create task exceeding max title length throws validation exception")]
    public async Task CreateTaskAsync_WithTitleExceedingMaxLength_ThrowsValidationException()
    {
        var request = new CreateTaskRequest(new string('Z', TaskConstants.MaxTitleLength + 1), TaskPriority.Low);

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateTaskAsync(request));
    }

    [Fact(DisplayName = "TC-APP-0004 - Get task by id returns matching task response")]
    public async Task GetTaskByIdAsync_WhenTaskExists_ReturnsTask()
    {
        var created = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Read documentation", TaskPriority.Low));

        var result = await _service.GetTaskByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Read documentation", result.Title);
    }

    [Fact(DisplayName = "TC-APP-0005 - Get task by non-existent hex id throws task not found exception")]
    public async Task GetTaskByIdAsync_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        const string nonExistentId = "0123456789abcdef01234567";

        await Assert.ThrowsAsync<TaskNotFoundException>(() => _service.GetTaskByIdAsync(nonExistentId));
    }

    [Theory(DisplayName = "TC-APP-0006 - Get task by malformed id throws invalid task id exception")]
    [InlineData("")]
    [InlineData("not-a-valid-id")]
    [InlineData("123")]
    [InlineData("zzzzzzzzzzzzzzzzzzzzzzzz")]
    public async Task GetTaskByIdAsync_WithInvalidIdFormat_ThrowsInvalidTaskIdException(string invalidId)
    {
        await Assert.ThrowsAsync<InvalidTaskIdException>(() => _service.GetTaskByIdAsync(invalidId));
    }

    [Fact(DisplayName = "TC-APP-0007 - List tasks filtered by status returns only matching items")]
    public async Task ListTasksAsync_FilterByStatus_ReturnsOnlyTasksWithMatchingStatus()
    {
        var task1 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Task 1", TaskPriority.Low));
        var task2 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Task 2", TaskPriority.Medium));
        var task3 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Task 3", TaskPriority.High));

        await _service.AdvanceTaskStatusAsync(task2.Id);
        await _service.AdvanceTaskStatusAsync(task3.Id);
        await _service.AdvanceTaskStatusAsync(task3.Id);

        var pendingTasks = await _service.ListTasksAsync(TaskStatus.Pending, null);
        var inProgressTasks = await _service.ListTasksAsync(TaskStatus.InProgress, null);
        var completedTasks = await _service.ListTasksAsync(TaskStatus.Completed, null);

        Assert.Single(pendingTasks);
        Assert.Equal(task1.Id, pendingTasks[0].Id);

        Assert.Single(inProgressTasks);
        Assert.Equal(task2.Id, inProgressTasks[0].Id);

        Assert.Single(completedTasks);
        Assert.Equal(task3.Id, completedTasks[0].Id);
    }

    [Fact(DisplayName = "TC-APP-0008 - List tasks searched by keyword returns case-insensitive matches")]
    public async Task ListTasksAsync_SearchByTitle_ReturnsCaseInsensitiveMatches()
    {
        await _service.CreateTaskAsync(TaskFaker.CreateRequest("Implement MongoDB repository", TaskPriority.High));
        await _service.CreateTaskAsync(TaskFaker.CreateRequest("Design React frontend", TaskPriority.Medium));
        await _service.CreateTaskAsync(TaskFaker.CreateRequest("Write mongodB index", TaskPriority.Low));

        var matches = await _service.ListTasksAsync(null, "mongodb");

        Assert.Equal(2, matches.Count);
        Assert.Contains(matches, t => t.Title == "Implement MongoDB repository");
        Assert.Contains(matches, t => t.Title == "Write mongodB index");
    }

    [Fact(DisplayName = "TC-APP-0009 - List tasks orders by priority high to low then by created date")]
    public async Task ListTasksAsync_Order_ReturnsHighPriorityFirstThenCreatedAt()
    {
        _timeProvider.SetUtcNow(_baseTime);
        var lowTask = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Low Task", TaskPriority.Low));

        _timeProvider.SetUtcNow(_baseTime.AddMinutes(5));
        var mediumTask = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Medium Task", TaskPriority.Medium));

        _timeProvider.SetUtcNow(_baseTime.AddMinutes(10));
        var highTask1 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("High Task 1", TaskPriority.High));

        _timeProvider.SetUtcNow(_baseTime.AddMinutes(15));
        var highTask2 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("High Task 2", TaskPriority.High));

        var list = await _service.ListTasksAsync(null, null);

        Assert.Equal(4, list.Count);
        Assert.Equal(highTask2.Id, list[0].Id);
        Assert.Equal(highTask1.Id, list[1].Id);
        Assert.Equal(mediumTask.Id, list[2].Id);
        Assert.Equal(lowTask.Id, list[3].Id);
    }

    [Fact(DisplayName = "TC-APP-0010 - Update task modifies title, priority and status correctly")]
    public async Task UpdateTaskAsync_UpdatesTitlePriorityAndStatus()
    {
        var created = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Initial Title", TaskPriority.Low));
        _timeProvider.Advance(TimeSpan.FromMinutes(20));

        var updateRequest = new UpdateTaskRequest(
            Title: "Refactored Title",
            Priority: TaskPriority.High,
            Status: TaskStatus.Completed);

        var updated = await _service.UpdateTaskAsync(created.Id, updateRequest);

        Assert.Equal("Refactored Title", updated.Title);
        Assert.Equal(TaskPriority.High, updated.Priority);
        Assert.Equal(TaskStatus.Completed, updated.Status);
        Assert.Equal(_baseTime.AddMinutes(20), updated.UpdatedAt);
    }

    [Fact(DisplayName = "TC-APP-0011 - Advance task status delegates lifecycle transition to domain")]
    public async Task AdvanceTaskStatusAsync_AdvancesStatusAccordingToDomainRules()
    {
        var task = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Advancement Test", TaskPriority.Medium));
        Assert.Equal(TaskStatus.Pending, task.Status);

        var step1 = await _service.AdvanceTaskStatusAsync(task.Id);
        Assert.Equal(TaskStatus.InProgress, step1.Status);

        var step2 = await _service.AdvanceTaskStatusAsync(task.Id);
        Assert.Equal(TaskStatus.Completed, step2.Status);

        var step3 = await _service.AdvanceTaskStatusAsync(task.Id);
        Assert.Equal(TaskStatus.Pending, step3.Status);
    }

    [Fact(DisplayName = "TC-APP-0012 - Delete existing task removes it from repository")]
    public async Task DeleteTaskAsync_WhenExists_RemovesTask()
    {
        var task = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Delete me", TaskPriority.Low));

        await _service.DeleteTaskAsync(task.Id);

        await Assert.ThrowsAsync<TaskNotFoundException>(() => _service.GetTaskByIdAsync(task.Id));
    }

    [Fact(DisplayName = "TC-APP-0013 - Delete non-existent task throws task not found exception")]
    public async Task DeleteTaskAsync_WhenNotFound_ThrowsTaskNotFoundException()
    {
        const string nonExistentId = "abcdef0123456789abcdef01";

        await Assert.ThrowsAsync<TaskNotFoundException>(() => _service.DeleteTaskAsync(nonExistentId));
    }

    [Fact(DisplayName = "TC-APP-0014 - Clear completed removes only completed tasks and returns count")]
    public async Task ClearCompletedTasksAsync_RemovesOnlyCompletedTasksAndReturnsCount()
    {
        var t1 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Pending Task", TaskPriority.Low));
        var t2 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Completed Task 1", TaskPriority.Medium));
        var t3 = await _service.CreateTaskAsync(TaskFaker.CreateRequest("Completed Task 2", TaskPriority.High));

        await _service.AdvanceTaskStatusAsync(t2.Id);
        await _service.AdvanceTaskStatusAsync(t2.Id);

        await _service.AdvanceTaskStatusAsync(t3.Id);
        await _service.AdvanceTaskStatusAsync(t3.Id);

        var clearedCount = await _service.ClearCompletedTasksAsync();

        Assert.Equal(2, clearedCount);

        var remaining = await _service.ListTasksAsync(null, null);
        Assert.Single(remaining);
        Assert.Equal(t1.Id, remaining[0].Id);
    }

    [Fact(DisplayName = "TC-APP-0015 - Summary with empty repository returns zeroes without division error")]
    public async Task GetSummaryAsync_WithEmptyList_ReturnsZeroTotalAndZeroProgressWithoutDivisionByZero()
    {
        var summary = await _service.GetSummaryAsync();

        Assert.NotNull(summary);
        Assert.Equal(0, summary.Total);
        Assert.Equal(0, summary.Pending);
        Assert.Equal(0, summary.InProgress);
        Assert.Equal(0, summary.Completed);
        Assert.Equal(0, summary.ProgressPercentage);
    }

    [Fact(DisplayName = "TC-APP-0016 - Summary with mixed tasks computes accurate counts and rounded percentage")]
    public async Task GetSummaryAsync_WithTasks_ReturnsAccurateCountersAndPercentage()
    {
        for (int i = 1; i <= 3; i++)
        {
            await _service.CreateTaskAsync(TaskFaker.CreateRequest($"Pending {i}", TaskPriority.Low));
        }

        for (int i = 1; i <= 2; i++)
        {
            var task = await _service.CreateTaskAsync(TaskFaker.CreateRequest($"In Progress {i}", TaskPriority.Medium));
            await _service.AdvanceTaskStatusAsync(task.Id);
        }

        for (int i = 1; i <= 3; i++)
        {
            var task = await _service.CreateTaskAsync(TaskFaker.CreateRequest($"Completed {i}", TaskPriority.High));
            await _service.AdvanceTaskStatusAsync(task.Id);
            await _service.AdvanceTaskStatusAsync(task.Id);
        }

        var summary = await _service.GetSummaryAsync();

        Assert.Equal(8, summary.Total);
        Assert.Equal(3, summary.Pending);
        Assert.Equal(2, summary.InProgress);
        Assert.Equal(3, summary.Completed);
        Assert.Equal(38, summary.ProgressPercentage);
    }
}
