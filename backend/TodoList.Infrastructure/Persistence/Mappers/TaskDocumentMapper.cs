using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Infrastructure.Persistence.Models;

namespace TodoList.Infrastructure.Persistence.Mappers;

public static class TaskDocumentMapper
{
    public static TodoTask ToDomain(TaskDocument doc)
    {
        var status = Enum.TryParse<TaskStatus>(doc.Status, ignoreCase: true, out var parsedStatus)
            ? parsedStatus
            : TaskStatus.Pending;

        var priority = Enum.TryParse<TaskPriority>(doc.Priority, ignoreCase: true, out var parsedPriority)
            ? parsedPriority
            : TaskPriority.Medium;

        var createdAt = new DateTimeOffset(doc.CreatedAt, TimeSpan.Zero);
        var updatedAt = doc.UpdatedAt.HasValue
            ? new DateTimeOffset(doc.UpdatedAt.Value, TimeSpan.Zero)
            : (DateTimeOffset?)null;

        return new TodoTask(doc.Id, doc.Title, priority, status, createdAt, updatedAt);
    }

    public static TaskDocument ToDocument(TodoTask entity)
    {
        return new TaskDocument
        {
            Id = entity.Id,
            Title = entity.Title,
            Status = entity.Status.ToString(),
            Priority = entity.Priority.ToString(),
            PriorityLevel = (int)entity.Priority,
            CreatedAt = entity.CreatedAt.UtcDateTime,
            UpdatedAt = entity.UpdatedAt?.UtcDateTime
        };
    }
}
