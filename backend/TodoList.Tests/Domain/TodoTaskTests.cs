using TodoList.Domain.Constants;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Exceptions;
using TodoList.Tests.Fakes;
using Xunit;

namespace TodoList.Tests.Domain;

public class TodoTaskTests
{
    private readonly DateTimeOffset _initialTime = new(2026, 10, 29, 12, 0, 0, TimeSpan.Zero);

    [Fact(DisplayName = "TC-DOM-0001 - Create valid task entity initializes with pending status")]
    public void Create_WithValidData_InitializesTaskWithPendingStatusAndCorrectProperties()
    {
        var task = TaskFaker.CreateTask("Buy groceries", TaskPriority.High, TaskStatus.Pending, _initialTime);

        Assert.NotNull(task.Id);
        Assert.Equal(24, task.Id.Length);
        Assert.Equal("Buy groceries", task.Title);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.Equal(_initialTime, task.CreatedAt);
        Assert.Null(task.UpdatedAt);
    }

    [Fact(DisplayName = "TC-DOM-0002 - Advance status cycles deterministically from pending to in progress to completed and back")]
    public void AdvanceStatus_FullCycle_TransitionsPendingToInProgressToCompletedBackToPending()
    {
        var task = TaskFaker.CreateTask("Cycle task", TaskPriority.Medium, TaskStatus.Pending, _initialTime);
        Assert.Equal(TaskStatus.Pending, task.Status);

        var step1Time = _initialTime.AddMinutes(5);
        var step2Time = _initialTime.AddMinutes(10);
        var step3Time = _initialTime.AddMinutes(15);

        task.AdvanceStatus(step1Time);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Equal(step1Time, task.UpdatedAt);

        task.AdvanceStatus(step2Time);
        Assert.Equal(TaskStatus.Completed, task.Status);
        Assert.Equal(step2Time, task.UpdatedAt);

        task.AdvanceStatus(step3Time);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.Equal(step3Time, task.UpdatedAt);
    }

    [Fact(DisplayName = "TC-DOM-0003 - Change priority updates priority value and updated timestamp")]
    public void ChangePriority_WithValidPriority_UpdatesPriorityAndUpdatedAt()
    {
        var task = TaskFaker.CreateTask(priority: TaskPriority.Low, createdAt: _initialTime);
        var updateTime = _initialTime.AddHours(1);

        task.ChangePriority(TaskPriority.High, updateTime);

        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal(updateTime, task.UpdatedAt);
    }

    [Fact(DisplayName = "TC-DOM-0004 - Change priority with undefined enum value throws domain validation exception")]
    public void ChangePriority_WithInvalidEnum_ThrowsDomainValidationException()
    {
        var task = TaskFaker.CreateTask(createdAt: _initialTime);

        Assert.Throws<DomainValidationException>(() => task.ChangePriority((TaskPriority)99, _initialTime.AddMinutes(1)));
    }

    [Fact(DisplayName = "TC-DOM-0005 - Change title trims whitespace and updates timestamp")]
    public void ChangeTitle_WithValidNewTitle_TrimsAndUpdatesTitleAndUpdatedAt()
    {
        var task = TaskFaker.CreateTask("Original title", createdAt: _initialTime);
        var updateTime = _initialTime.AddMinutes(30);

        task.ChangeTitle("   Updated title with spaces   ", updateTime);

        Assert.Equal("Updated title with spaces", task.Title);
        Assert.Equal(updateTime, task.UpdatedAt);
    }

    [Theory(DisplayName = "TC-DOM-0006 - Change title with empty or whitespace throws domain validation exception")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ChangeTitle_WhenNullOrWhitespace_ThrowsDomainValidationException(string? invalidTitle)
    {
        var task = TaskFaker.CreateTask("Valid title", createdAt: _initialTime);

        Assert.Throws<DomainValidationException>(() => task.ChangeTitle(invalidTitle!, _initialTime));
    }

    [Fact(DisplayName = "TC-DOM-0007 - Change title exceeding max length throws domain validation exception")]
    public void ChangeTitle_WhenExceedsMaxTitleLength_ThrowsDomainValidationException()
    {
        var task = TaskFaker.CreateTask(createdAt: _initialTime);
        var longTitle = new string('A', TaskConstants.MaxTitleLength + 1);

        var exception = Assert.Throws<DomainValidationException>(() => task.ChangeTitle(longTitle, _initialTime));
        Assert.Contains(TaskConstants.MaxTitleLength.ToString(), exception.Message);
    }

    [Theory(DisplayName = "TC-DOM-0008 - Create with empty or whitespace title throws domain validation exception")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WhenTitleIsEmptyOrWhitespace_ThrowsDomainValidationException(string? invalidTitle)
    {
        Assert.Throws<DomainValidationException>(() => TodoTask.Create(invalidTitle!, TaskPriority.Medium, _initialTime));
    }

    [Fact(DisplayName = "TC-DOM-0009 - Create with title exceeding max length throws domain validation exception")]
    public void Create_WhenTitleExceedsMaxTitleLength_ThrowsDomainValidationException()
    {
        var longTitle = new string('x', TaskConstants.MaxTitleLength + 1);

        Assert.Throws<DomainValidationException>(() => TodoTask.Create(longTitle, TaskPriority.Medium, _initialTime));
    }

    [Fact(DisplayName = "TC-DOM-0010 - Change status to explicit valid state updates status and timestamp")]
    public void ChangeStatus_WithValidStatus_UpdatesStatusAndUpdatedAt()
    {
        var task = TaskFaker.CreateTask(createdAt: _initialTime);
        var updateTime = _initialTime.AddHours(2);

        task.ChangeStatus(TaskStatus.Completed, updateTime);

        Assert.Equal(TaskStatus.Completed, task.Status);
        Assert.Equal(updateTime, task.UpdatedAt);
    }

    [Fact(DisplayName = "TC-DOM-0011 - Change status to invalid enum throws domain validation exception")]
    public void ChangeStatus_WithInvalidStatus_ThrowsDomainValidationException()
    {
        var task = TaskFaker.CreateTask(createdAt: _initialTime);

        Assert.Throws<DomainValidationException>(() => task.ChangeStatus((TaskStatus)99, _initialTime.AddMinutes(1)));
    }
}
