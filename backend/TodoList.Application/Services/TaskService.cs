using TodoList.Application.DTOs;
using TodoList.Application.Exceptions;
using TodoList.Application.Interfaces;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly TimeProvider _timeProvider;

    public TaskService(ITaskRepository repository, TimeProvider timeProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public async Task<IReadOnlyList<TaskResponse>> ListTasksAsync(
        TaskStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _repository.ListAsync(status, search, cancellationToken);
        return tasks.Select(TaskResponse.FromDomain).ToList();
    }

    public async Task<TaskResponse> GetTaskByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateTaskId(id);
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            throw new TaskNotFoundException(id);
        }

        return TaskResponse.FromDomain(task);
    }

    public async Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException(nameof(request.Title), "Task title cannot be empty or whitespace.");
        }

        var trimmedTitle = request.Title.Trim();
        if (trimmedTitle.Length < TaskConstants.MinTitleLength || trimmedTitle.Length > TaskConstants.MaxTitleLength)
        {
            throw new ValidationException(
                nameof(request.Title),
                $"Task title length must be between {TaskConstants.MinTitleLength} and {TaskConstants.MaxTitleLength} characters.");
        }

        var now = _timeProvider.GetUtcNow();
        var task = TodoTask.Create(trimmedTitle, request.Priority, now);
        var created = await _repository.AddAsync(task, cancellationToken);
        return TaskResponse.FromDomain(created);
    }

    public async Task<TaskResponse> UpdateTaskAsync(
        string id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateTaskId(id);
        ArgumentNullException.ThrowIfNull(request);

        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            throw new TaskNotFoundException(id);
        }

        var now = _timeProvider.GetUtcNow();

        if (request.Title is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ValidationException(nameof(request.Title), "Task title cannot be empty or whitespace.");
            }

            var trimmedTitle = request.Title.Trim();
            if (trimmedTitle.Length < TaskConstants.MinTitleLength || trimmedTitle.Length > TaskConstants.MaxTitleLength)
            {
                throw new ValidationException(
                    nameof(request.Title),
                    $"Task title length must be between {TaskConstants.MinTitleLength} and {TaskConstants.MaxTitleLength} characters.");
            }

            task.ChangeTitle(trimmedTitle, now);
        }

        if (request.Priority.HasValue)
        {
            task.ChangePriority(request.Priority.Value, now);
        }

        if (request.Status.HasValue)
        {
            task.ChangeStatus(request.Status.Value, now);
        }

        var updated = await _repository.UpdateAsync(task, cancellationToken);
        return TaskResponse.FromDomain(updated);
    }

    public async Task<TaskResponse> AdvanceTaskStatusAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateTaskId(id);
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            throw new TaskNotFoundException(id);
        }

        var now = _timeProvider.GetUtcNow();
        task.AdvanceStatus(now);

        var updated = await _repository.UpdateAsync(task, cancellationToken);
        return TaskResponse.FromDomain(updated);
    }

    public async Task DeleteTaskAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateTaskId(id);
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new TaskNotFoundException(id);
        }
    }

    public async Task<int> ClearCompletedTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.ClearCompletedAsync(cancellationToken);
    }

    public async Task<TaskSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var counts = await _repository.GetStatusCountsAsync(cancellationToken);
        var pending = counts.GetValueOrDefault(TaskStatus.Pending, 0);
        var inProgress = counts.GetValueOrDefault(TaskStatus.InProgress, 0);
        var completed = counts.GetValueOrDefault(TaskStatus.Completed, 0);
        var total = pending + inProgress + completed;

        var progressPercentage = total == 0
            ? 0
            : (int)Math.Round((double)completed / total * 100, MidpointRounding.AwayFromZero);

        return new TaskSummaryResponse(total, pending, inProgress, completed, progressPercentage);
    }

    private static void ValidateTaskId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidTaskIdException(id, "Task ID cannot be null or whitespace.");
        }

        var trimmed = id.Trim();
        if (trimmed.Length != 24 || !trimmed.All(Uri.IsHexDigit))
        {
            throw new InvalidTaskIdException(trimmed, "Task ID must be a 24-character hexadecimal string.");
        }
    }
}
