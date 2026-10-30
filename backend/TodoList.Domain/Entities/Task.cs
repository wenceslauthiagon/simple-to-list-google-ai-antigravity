using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public class Task : TodoTask
{
    public Task(
        string id,
        string title,
        TaskPriority priority,
        TaskStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt = null)
        : base(id, title, priority, status, createdAt, updatedAt)
    {
    }

    public new static Task Create(
        string title,
        TaskPriority priority,
        DateTimeOffset createdAt,
        string? id = null)
    {
        var todoTask = TodoTask.Create(title, priority, createdAt, id);
        return new Task(todoTask.Id, todoTask.Title, todoTask.Priority, todoTask.Status, todoTask.CreatedAt, todoTask.UpdatedAt);
    }
}
