using System.Security.Cryptography;
using TodoList.Domain.Constants;
using TodoList.Domain.Enums;
using TodoList.Domain.Exceptions;

namespace TodoList.Domain.Entities;

public class TodoTask
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public TodoTask(
        string id,
        string title,
        TaskPriority priority,
        TaskStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new DomainValidationException("Task ID cannot be null or empty.");
        }

        Id = id.Trim();
        Title = ValidateAndNormalizeTitle(title);
        Priority = ValidatePriority(priority);
        Status = ValidateStatus(status);
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static TodoTask Create(
        string title,
        TaskPriority priority,
        DateTimeOffset createdAt,
        string? id = null)
    {
        var taskId = string.IsNullOrWhiteSpace(id) ? GenerateId() : id.Trim();
        return new TodoTask(taskId, title, priority, TaskStatus.Pending, createdAt, null);
    }

    public void ChangeTitle(string newTitle, DateTimeOffset updatedAt)
    {
        Title = ValidateAndNormalizeTitle(newTitle);
        UpdatedAt = updatedAt;
    }

    public void ChangePriority(TaskPriority newPriority, DateTimeOffset updatedAt)
    {
        Priority = ValidatePriority(newPriority);
        UpdatedAt = updatedAt;
    }

    public void ChangeStatus(TaskStatus newStatus, DateTimeOffset updatedAt)
    {
        Status = ValidateStatus(newStatus);
        UpdatedAt = updatedAt;
    }

    public void AdvanceStatus(DateTimeOffset updatedAt)
    {
        Status = Status switch
        {
            TaskStatus.Pending => TaskStatus.InProgress,
            TaskStatus.InProgress => TaskStatus.Completed,
            TaskStatus.Completed => TaskStatus.Pending,
            _ => TaskStatus.Pending
        };

        UpdatedAt = updatedAt;
    }

    private static string ValidateAndNormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainValidationException("Task title cannot be null, empty, or whitespace.");
        }

        var trimmed = title.Trim();
        if (trimmed.Length < TaskConstants.MinTitleLength || trimmed.Length > TaskConstants.MaxTitleLength)
        {
            throw new DomainValidationException(
                $"Task title length must be between {TaskConstants.MinTitleLength} and {TaskConstants.MaxTitleLength} characters.");
        }

        return trimmed;
    }

    private static TaskPriority ValidatePriority(TaskPriority priority)
    {
        if (!Enum.IsDefined(typeof(TaskPriority), priority))
        {
            throw new DomainValidationException($"Invalid task priority: {priority}");
        }

        return priority;
    }

    private static TaskStatus ValidateStatus(TaskStatus status)
    {
        if (!Enum.IsDefined(typeof(TaskStatus), status))
        {
            throw new DomainValidationException($"Invalid task status: {status}");
        }

        return status;
    }

    private static string GenerateId()
    {
        var bytes = new byte[12];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
